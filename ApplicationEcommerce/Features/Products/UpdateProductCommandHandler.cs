using ApplicationEcommerce.Common.Exceptions;
using ApplicationEcommerce.Interfaces;
using Domain_Ecommerce.Entity;
using Domain_Ecommerce.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationEcommerce.Features.Products
{
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateProductCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // نفس التعيين المستخدم في CreateProductCommandHandler، لازم يتطابق تمامًا
        private static readonly Dictionary<string, VariantSize> SizeMap = new()
        {
            { "20x30", VariantSize.Small },
            { "30x40", VariantSize.Medium },
            { "40x50", VariantSize.Large },
        };

        // نفس الألوان المستخدمة في CreateProductCommandHandler، لازم تتطابق تمامًا
        private const string NoColor = "-";
        private static readonly string[] AllowedColors = { "Black", "White" };

        private static List<string> ResolveColors(List<string>? requested)
        {
            if (requested is null || requested.Count == 0)
                return new List<string> { NoColor };

            var result = new List<string>();

            foreach (var text in requested)
            {
                var canonical = AllowedColors.FirstOrDefault(
                        c => c.Equals(text?.Trim(), StringComparison.OrdinalIgnoreCase))
                    ?? throw new BadRequestException($"اللون غير مدعوم: {text}");

                if (!result.Contains(canonical))
                    result.Add(canonical);
            }

            return result;
        }

        private static string BuildSku(Guid productId, string sizeText, string color) =>
            color == NoColor
                ? $"{productId}-{sizeText}"
                : $"{productId}-{sizeText}-{color}";

        public async Task Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(request.Id)
                ?? throw new NotFoundException(nameof(Product), request.Id);

            // نتحقق من الألوان قبل أي تعديل
            var colors = ResolveColors(request.Colors);

            product.Name = request.Name;
            product.Description = request.Description;
            product.CategoryId = request.CategoryId;
            product.Gender = request.Gender;
            product.BasePrice = request.BasePrice;
            product.IsNew = request.IsNew;
            product.IsOnSale = request.IsOnSale;
            product.IsCustomizable = request.IsCustomizable;
            _unitOfWork.Repository<Product>().Update(product);

            var existingVariants = (await _unitOfWork.Repository<ProductVariant>()
                .FindAsync(v => v.ProductId == product.Id)).ToList();

            if (request.Sizes is not null)
            {
                foreach (var sizeText in request.Sizes)
                {
                    if (!SizeMap.TryGetValue(sizeText, out var parsedSize))
                        continue;

                    var price = request.SizePrices is not null && request.SizePrices.TryGetValue(sizeText, out var p)
                        ? p
                        : request.BasePrice;

                    foreach (var color in colors)
                    {
                        // 1) نفس المقاس ونفس اللون: نعدّل السعر بس
                        var match = existingVariants.FirstOrDefault(v => v.Size == parsedSize && v.Color == color);

                        // 2) الـ variant القديم (لون "-") نحوّله للون الحقيقي بدل ما نسيبه ونضيف جديد،
                        //    ونحافظ على الـ Id بتاعه عشان الأوردرات والسلات اللي عليه
                        if (match is null && color != NoColor)
                        {
                            match = existingVariants.FirstOrDefault(v => v.Size == parsedSize && v.Color == NoColor);
                            if (match is not null)
                            {
                                match.Color = color;
                                match.SKU = BuildSku(product.Id, sizeText, color);
                            }
                        }

                        if (match is not null)
                        {
                            match.Price = price;
                            _unitOfWork.Repository<ProductVariant>().Update(match);
                        }
                        else
                        {
                            var variant = new ProductVariant
                            {
                                ProductId = product.Id,
                                Color = color,
                                Size = parsedSize,
                                Price = price,
                                StockQuantity = 100,
                                SKU = BuildSku(product.Id, sizeText, color)
                            };

                            await _unitOfWork.Repository<ProductVariant>().AddAsync(variant);
                            existingVariants.Add(variant);
                        }
                    }
                }
            }

            await _unitOfWork.SaveChangesAsync();
        }
    }
}