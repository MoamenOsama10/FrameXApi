using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationEcommerce.Features.Wishlist
{
    public record AddToWishlistCommand(string UserId, Guid ProductId) : IRequest;
}
