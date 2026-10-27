using ApplicationEcommerce.Common.Exceptions;
using ApplicationEcommerce.Interfaces;
using Domain_Ecommerce.Entity;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationEcommerce.Features.Cart
{
    public class AddItemToCartCommandHandler : IRequestHandler<AddItemToCartCommand, Guid>
    {
        private readonly ICartRepository _cartRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<ProductVariant> _variantRepository;

        public AddItemToCartCommandHandler(
            ICartRepository cartRepository,
            IUnitOfWork unitOfWork,
            IGenericRepository<ProductVariant> variantRepository)
        {
            _cartRepository = cartRepository;
            _unitOfWork = unitOfWork;
            _variantRepository = variantRepository;
        }

        public async Task<Guid> Handle(AddItemToCartCommand request, CancellationToken cancellationToken)
        {
            var variant = await _variantRepository.GetByIdAsync(request.ProductVariantId)
                ?? throw new NotFoundException(nameof(ProductVariant), request.ProductVariantId);

            if (variant.StockQuantity < request.Quantity)
                throw new BadRequestException("Requested quantity exceeds available stock.");

            var cart = await _cartRepository.GetByUserIdAsync(request.UserId);

            if (cart is null)
            {
                cart = new Domain_Ecommerce.Entity.Cart { UserId = request.UserId };
                await _cartRepository.AddAsync(cart);
            }

            var existingItem = cart.Items.FirstOrDefault(i => i.ProductVariantId == request.ProductVariantId);

            if (existingItem is not null)
            {
                existingItem.Quantity += request.Quantity;
            }
            else
            {
                cart.Items.Add(new CartItem
                {
                    CartId = cart.Id,
                    ProductVariantId = request.ProductVariantId,
                    Quantity = request.Quantity
                });
            }

            await _unitOfWork.SaveChangesAsync();

            return cart.Id;
        }
    }
}
