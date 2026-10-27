using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationEcommerce.Features.Cart
{
    public record AddItemToCartCommand(
    string UserId,
    Guid ProductVariantId,
    int Quantity) : IRequest<Guid>;
}
