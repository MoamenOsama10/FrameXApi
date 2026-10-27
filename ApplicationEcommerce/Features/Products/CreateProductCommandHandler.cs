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
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateProductCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // تعيين ثابت بين المقاس النصي القادم من الفرونت اند وقيمة الـ enum
        private static readonly Dictionary<string, VariantSize> SizeMap = new()
        {
            { "20x30", VariantSize.Small },
            { "30x40", VariantSize.Medium },
            { "40x50", VariantSize.Large },
        };

        // الألوان المسموحة، لازم تتطابق مع القايمة في الفورم
        private const string NoColor = "-";
        private static readonly string[] AllowedColors = { "Black", "White" };

        private static List<string> ResolveColors(List<string>? requested)
        {
            // لو مفيش ألوان، نفس السلوك القديم: variant واحد لكل مقاس بلون "-"
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

        public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            // نتحقق من الألوان قبل ما نحفظ أي حاجة، عشان لون غلط ميسيبش منتج نص نص
            var colors = ResolveColors(request.Colors);

            var product = new Product
            {
                Name = request.Name,
                Description = request.Description,
                CategoryId = request.CategoryId,
                Gender = request.Gender,
                BasePrice = request.BasePrice,
                IsNew = request.IsNew,
                IsOnSale = request.IsOnSale,
                IsCustomizable = request.IsCustomizable,
            };

            await _unitOfWork.Repository<Product>().AddAsync(product);
            await _unitOfWork.SaveChangesAsync();

            if (request.Sizes is not null && request.Sizes.Count > 0)
            {
                foreach (var sizeText in request.Sizes)
                {
                    if (!SizeMap.TryGetValue(sizeText, out var parsedSize))
                        continue; // مقاس غير معروف، تخطاه

                    var price = request.SizePrices is not null && request.SizePrices.TryGetValue(sizeText, out var p)
                        ? p
                        : request.BasePrice;

                    // variant لكل تركيبة (مقاس × لون)، والألوان بتاخد نفس سعر المقاس
                    foreach (var color in colors)
                    {
                        var variant = new ProductVariant
                        {
                            ProductId = product.Id,
                            Color = color,
                            Size = parsedSize,
                            Price = price,
                            StockQuantity = 100,
                            SKU = color == NoColor
                                ? $"{product.Id}-{sizeText}"
                                : $"{product.Id}-{sizeText}-{color}"
                        };

                        await _unitOfWork.Repository<ProductVariant>().AddAsync(variant);
                    }
                }

                await _unitOfWork.SaveChangesAsync();
            }

            return product.Id;
        }
    }
}