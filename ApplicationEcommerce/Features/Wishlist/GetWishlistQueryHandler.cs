using ApplicationEcommerce.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationEcommerce.Features.Wishlist
{
    public class GetWishlistQueryHandler : IRequestHandler<GetWishlistQuery, List<WishlistItemDto>>
    {
        private readonly IWishlistRepository _wishlistRepository;

        public GetWishlistQueryHandler(IWishlistRepository wishlistRepository)
        {
            _wishlistRepository = wishlistRepository;
        }

        public async Task<List<WishlistItemDto>> Handle(GetWishlistQuery request, CancellationToken cancellationToken)
        {
            var wishlist = await _wishlistRepository.GetByUserIdAsync(request.UserId);

            if (wishlist is null)
                return new List<WishlistItemDto>();

            return wishlist.Items.Select(i => new WishlistItemDto
            {
                Id = i.Id,
                ProductId = i.ProductId,
                ProductName = i.Product.Name,
                Price = i.Product.BasePrice,
                ImageUrl = i.Product.Variants
                    .FirstOrDefault()?.Images.FirstOrDefault(img => img.IsMain)?.ImageUrl
            }).ToList();
        }
    }
}
