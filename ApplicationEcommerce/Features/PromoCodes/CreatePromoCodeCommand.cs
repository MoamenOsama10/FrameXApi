using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationEcommerce.Features.PromoCodes
{
    public record CreatePromoCodeCommand(string Code, decimal DiscountPercent, int UsageLimit) : IRequest<Guid>;
}
