using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationEcommerce.Features.PromoCodes
{
    public class CreatePromoCodeCommandValidator : AbstractValidator<CreatePromoCodeCommand>
    {
        public CreatePromoCodeCommandValidator()
        {
            RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
            RuleFor(x => x.DiscountPercent).InclusiveBetween(1, 100);
            RuleFor(x => x.UsageLimit).GreaterThan(0);
        }
    }
}
