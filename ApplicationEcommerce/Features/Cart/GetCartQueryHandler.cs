using ApplicationEcommerce.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationEcommerce.Features.Cart
{
    public class GetCartQueryHandler : IRequestHandler<GetCartQuery, CartDto>
    {
        private readonly ICartRepository _cartRepository;

        public GetCartQueryHandler(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        public async Task<CartDto> Handle(GetCartQuery request, CancellationToken cancellationToken)
        {
            var cart = await _cartRepository.GetByUserIdAsync(request.UserId);

            if (cart is null)
                return new CartDto();

            return new CartDto
            {
                Id = cart.Id,
                Items = cart.Items.Select(i => new CartItemDto
                {
                    Id = i.Id,
                    ProductVariantId = i.ProductVariantId,
                    ProductName = i.ProductVariant.Product.Name,
                    Color = i.ProductVariant.Color,
                    Size = i.ProductVariant.Size.ToString(),
                    Price = i.ProductVariant.Price,
                    Quantity = i.Quantity,
                    ImageUrl = i.ProductVariant.Images.FirstOrDefault(img => img.IsMain)?.ImageUrl
                }).ToList()
            };
        }
    }
}
