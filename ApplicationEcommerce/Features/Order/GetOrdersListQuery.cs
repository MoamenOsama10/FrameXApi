using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationEcommerce.Features.Order
{
    public record GetOrdersListQuery(string UserId) : IRequest<List<OrderDto>>;
}
