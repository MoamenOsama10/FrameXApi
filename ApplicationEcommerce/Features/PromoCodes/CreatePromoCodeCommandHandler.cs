using ApplicationEcommerce.Common.Exceptions;
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
    public class CreatePromoCodeCommandHandler : IRequestHandler<CreatePromoCodeCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreatePromoCodeCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(CreatePromoCodeCommand request, CancellationToken cancellationToken)
        {
            var existing = await _unitOfWork.Repository<PromoCode>()
                .FindAsync(p => p.Code == request.Code.ToUpper());

            if (existing.Any())
                throw new BadRequestException("Promo code already exists.");

            var promoCode = new PromoCode
            {
                Code = request.Code.ToUpper(),
                DiscountPercent = request.DiscountPercent,
                UsageLimit = request.UsageLimit
            };

            await _unitOfWork.Repository<PromoCode>().AddAsync(promoCode);
            await _unitOfWork.SaveChangesAsync();

            return promoCode.Id;
        }
    }
}
