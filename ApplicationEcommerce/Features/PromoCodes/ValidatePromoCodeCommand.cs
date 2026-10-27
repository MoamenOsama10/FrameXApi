using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationEcommerce.Features.PromoCodes
{
    public record ValidatePromoCodeCommand(string Code) : IRequest<ValidatePromoCodeResult>;

    public record ValidatePromoCodeResult(bool IsValid, decimal DiscountPercent, string? Error);
}
