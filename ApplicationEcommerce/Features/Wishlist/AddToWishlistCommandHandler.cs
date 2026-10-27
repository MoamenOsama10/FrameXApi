using ApplicationEcommerce.Interfaces;
using Domain_Ecommerce.Entity;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationEcommerce.Features.Wishlist
{
    public class AddToWishlistCommandHandler : IRequestHandler<AddToWishlistCommand>
    {
        private readonly IWishlistRepository _wishlistRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AddToWishlistCommandHandler(IWishlistRepository wishlistRepository, IUnitOfWork unitOfWork)
        {
            _wishlistRepository = wishlistRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(AddToWishlistCommand request, CancellationToken cancellationToken)
        {
            var wishlist = await _wishlistRepository.GetByUserIdAsync(request.UserId);

            if (wishlist is null)
            {
                wishlist = new Domain_Ecommerce.Entity.Wishlist { UserId = request.UserId };
                await _wishlistRepository.AddAsync(wishlist);
            }

            if (wishlist.Items.Any(i => i.ProductId == request.ProductId))
                return; // already exists, idempotent

            wishlist.Items.Add(new WishlistItem
            {
                WishlistId = wishlist.Id,
                ProductId = request.ProductId
            });

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
