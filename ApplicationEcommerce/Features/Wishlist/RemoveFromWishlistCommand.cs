using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationEcommerce.Features.Wishlist
{
    public record RemoveFromWishlistCommand(string UserId, Guid ProductId) : IRequest;
}
