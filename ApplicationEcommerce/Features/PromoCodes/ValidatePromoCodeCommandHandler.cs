using ApplicationEcommerce.Interfaces;
using Domain_Ecommerce.Entity;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationEcommerce.Features.PromoCodes
{
    public class ValidatePromoCodeCommandHandler : IRequestHandler<ValidatePromoCodeCommand, ValidatePromoCodeResult>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ValidatePromoCodeCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ValidatePromoCodeResult> Handle(ValidatePromoCodeCommand request, CancellationToken cancellationToken)
{
    var matches = await _unitOfWork.Repository<PromoCode>()
        .FindAsync(p => p.Code == request.Code.ToUpper() && p.IsActive);

    var promoCode = matches.FirstOrDefault();

    if (promoCode is null)
        return new ValidatePromoCodeResult(false, 0, "Invalid promo code.");

    if (promoCode.UsedCount >= promoCode.UsageLimit)
        return new ValidatePromoCodeResult(false, 0, "This promo code has reached its usage limit.");

    return new ValidatePromoCodeResult(true, promoCode.DiscountPercent, null);
}
    }
}
