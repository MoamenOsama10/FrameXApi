using System.Globalization;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using MediatR;
using Microsoft.Extensions.Logging;
using ApplicationEcommerce.Common.Exceptions;
using ApplicationEcommerce.Interfaces;
using Domain_Ecommerce.Entity;
using Domain_Ecommerce.Enums;
namespace ApplicationEcommerce.Features.GuestCheckout.Commands;

public class GuestCheckoutCommandHandler : IRequestHandler<GuestCheckoutCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;
    private readonly ILogger<GuestCheckoutCommandHandler> _logger;

    public GuestCheckoutCommandHandler(
        IUnitOfWork unitOfWork,
        IEmailService emailService,
        ILogger<GuestCheckoutCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _emailService = emailService;
        _logger = logger;
    }

    // نفس التعيين المستخدم وقت إنشاء المنتج، لازم يتطابق تمامًا
    private static readonly Dictionary<string, VariantSize> SizeMap = new()
    {
        { "20x30", VariantSize.Small },
        { "30x40", VariantSize.Medium },
        { "40x50", VariantSize.Large },
    };

    // رابط صورة العميل لازم يكون بالشكل اللي endpoint الرفع بيرجّعه بالظبط
    private static readonly Regex CustomImageUrlPattern =
        new(@"^/uploads/custom/[0-9a-f]{32}\.(jpg|jpeg|png|webp)$", RegexOptions.Compiled);

    public async Task<Guid> Handle(GuestCheckoutCommand request, CancellationToken cancellationToken)
    {
        if (request.Items == null || request.Items.Count == 0)
            throw new BadRequestException("السلة فارغة.");

        // نجهّز الـ items من بيانات الـ database (السعر والاسم من السيرفر مش من العميل)
        var resolvedItems = new List<(Guid VariantId, string Name, string? Size, int Quantity, decimal Price, string? CustomImageUrl)>();

        foreach (var item in request.Items)
        {
            if (item.Quantity < 1 || item.Quantity > 50)
                throw new BadRequestException("الكمية غير صالحة.");

            // الـ Query Filter بيرجّع null للمنتج المحذوف
            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(item.ProductId)
                ?? throw new BadRequestException("أحد المنتجات في طلبك لم يعد متاحاً.");

            // المنتج المخصص لازم يكون معاه صورة العميل، والمنتجات العادية بتتجاهل أي رابط
            string? customImageUrl = null;
            if (product.IsCustomizable)
            {
                if (string.IsNullOrWhiteSpace(item.CustomImageUrl)
                    || !CustomImageUrlPattern.IsMatch(item.CustomImageUrl))
                {
                    throw new BadRequestException($"من فضلك ارفع صورتك للمنتج {product.Name}.");
                }

                customImageUrl = item.CustomImageUrl;
            }

            var variants = await _unitOfWork.Repository<ProductVariant>()
                .FindAsync(v => v.ProductId == item.ProductId);

            ProductVariant? variant;

            if (string.IsNullOrWhiteSpace(item.Size))
            {
                variant = variants.FirstOrDefault();
            }
            else if (SizeMap.TryGetValue(item.Size, out var mappedSize))
            {
                variant = variants.FirstOrDefault(v => v.Size == mappedSize);
            }
            else
            {
                variant = null;
            }

            if (variant is null)
                throw new BadRequestException($"المقاس المطلوب غير متاح للمنتج {product.Name}.");

            var sizeLabel = SizeMap.FirstOrDefault(kv => kv.Value == variant.Size).Key;

            resolvedItems.Add((variant.Id, product.Name, sizeLabel, item.Quantity, variant.Price, customImageUrl));
        }

        var subtotal = resolvedItems.Sum(i => i.Price * i.Quantity);
        var shipping = request.ShippingCost ?? (subtotal >= 1100 ? 0 : 60);
        decimal total = subtotal + shipping;

        if (!string.IsNullOrWhiteSpace(request.PromoCode))
        {
            var matches = await _unitOfWork.Repository<PromoCode>()
                .FindAsync(p => p.Code == request.PromoCode.ToUpper() && p.IsActive);

            var promoCode = matches.FirstOrDefault();

            if (promoCode is not null && promoCode.UsedCount < promoCode.UsageLimit)
            {
                total -= subtotal * promoCode.DiscountPercent / 100;
                promoCode.UsedCount++;
                _unitOfWork.Repository<PromoCode>().Update(promoCode);
            }
        }

        var order = new Domain_Ecommerce.Entity.Order
        {
            UserId = "guest",
            OrderNumber = $"FX{DateTime.UtcNow:yyMMdd}{Random.Shared.Next(1000, 9999)}",
            Status = OrderStatus.Pending,
            Subtotal = subtotal,
            Shipping = shipping,
            Tax = 0,
            Total = total,
            CustomerName = request.CustomerName,
            CustomerEmail = request.CustomerEmail,
            CustomerPhone = request.CustomerPhone,
            City = request.City,
            AddressDetails = request.AddressDetails,
            Notes = request.Notes,
            DepositAccount = request.DepositAccount,
            DepositReceiptUrl = request.DepositReceiptUrl
        };

        foreach (var r in resolvedItems)
        {
            order.Items.Add(new OrderItem
            {
                ProductVariantId = r.VariantId,
                Quantity = r.Quantity,
                UnitPrice = r.Price,
                CustomImageUrl = r.CustomImageUrl
            });
        }

        var receiptItems = resolvedItems
            .Select(r => (r.Name, r.Size, r.Quantity, r.Price))
            .ToList();

        await _unitOfWork.Repository<Domain_Ecommerce.Entity.Order>().AddAsync(order);
        await _unitOfWork.SaveChangesAsync();

        // إيميل الإيصال: لو فشل، الأوردر يفضل ناجح
        if (!string.IsNullOrWhiteSpace(order.CustomerEmail))
        {
            try
            {
                await _emailService.SendEmailAsync(
                    order.CustomerEmail,
                    $"إيصال طلبك رقم {order.OrderNumber} - FrameX",
                    BuildReceiptHtml(order, receiptItems));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send receipt email for order {OrderNumber}", order.OrderNumber);
            }
        }

        return order.Id;
    }

    private static string BuildReceiptHtml(
        Domain_Ecommerce.Entity.Order order,
        List<(string Name, string? Size, int Quantity, decimal Price)> items)
    {
        static string Money(decimal v) => v.ToString("0.00", CultureInfo.InvariantCulture);
        static string Enc(string? s) => WebUtility.HtmlEncode(s ?? string.Empty);

        var deposit = order.Total * 0.3m;
        var remaining = order.Total - deposit;
        var discount = order.Subtotal + order.Shipping - order.Total;

        var rows = new StringBuilder();
        foreach (var i in items)
        {
            var size = string.IsNullOrWhiteSpace(i.Size) ? "" : $" ({Enc(i.Size)})";
            rows.Append($"<tr>" +
                $"<td style='padding:8px;border-bottom:1px solid #eee'>{Enc(i.Name)}{size}</td>" +
                $"<td style='padding:8px;border-bottom:1px solid #eee;text-align:center'>{i.Quantity}</td>" +
                $"<td style='padding:8px;border-bottom:1px solid #eee;text-align:left'>{Money(i.Price * i.Quantity)} جنيه</td>" +
                $"</tr>");
        }

        var discountRow = discount > 0
            ? $"<tr><td colspan='2' style='padding:6px 8px'>الخصم</td><td style='padding:6px 8px;text-align:left'>- {Money(discount)} جنيه</td></tr>"
            : "";

        return
            $"<div dir='rtl' style='font-family:Tahoma,Arial,sans-serif;max-width:600px;margin:auto;color:#111'>" +
            $"<h2 style='margin-bottom:4px'>شكراً لطلبك من FrameX</h2>" +
            $"<p style='margin-top:0;color:#555'>أهلاً {Enc(order.CustomerName)}، دي تفاصيل طلبك.</p>" +
            $"<p><strong>رقم الطلب:</strong> {Enc(order.OrderNumber)}</p>" +
            $"<table style='width:100%;border-collapse:collapse;font-size:14px'>" +
            $"<thead><tr style='background:#f5f5f5'>" +
            $"<th style='padding:8px;text-align:right'>المنتج</th>" +
            $"<th style='padding:8px'>الكمية</th>" +
            $"<th style='padding:8px;text-align:left'>السعر</th></tr></thead>" +
            $"<tbody>{rows}</tbody>" +
            $"<tfoot>" +
            $"<tr><td colspan='2' style='padding:6px 8px'>المجموع الفرعي</td><td style='padding:6px 8px;text-align:left'>{Money(order.Subtotal)} جنيه</td></tr>" +
            $"<tr><td colspan='2' style='padding:6px 8px'>الشحن</td><td style='padding:6px 8px;text-align:left'>{Money(order.Shipping)} جنيه</td></tr>" +
            discountRow +
            $"<tr><td colspan='2' style='padding:8px;font-weight:bold'>إجمالي الطلب</td><td style='padding:8px;text-align:left;font-weight:bold'>{Money(order.Total)} جنيه</td></tr>" +
            $"<tr><td colspan='2' style='padding:6px 8px;color:#15803d'>المقدم المدفوع (30%)</td><td style='padding:6px 8px;text-align:left;color:#15803d'>{Money(deposit)} جنيه</td></tr>" +
            $"<tr><td colspan='2' style='padding:6px 8px'>المتبقي عند الاستلام</td><td style='padding:6px 8px;text-align:left'>{Money(remaining)} جنيه</td></tr>" +
            $"</tfoot></table>" +
            $"<p style='margin-top:20px;font-size:13px;color:#555'>العنوان: {Enc(order.City)} - {Enc(order.AddressDetails)}</p>" +
            $"<p style='font-size:13px;color:#555'>هنراجع التحويل ونتواصل معاك لتأكيد الشحن.</p>" +
            $"</div>";
    }
}