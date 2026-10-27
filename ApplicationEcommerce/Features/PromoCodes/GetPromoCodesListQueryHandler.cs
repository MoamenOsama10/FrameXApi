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
    public class GetPromoCodesListQueryHandler : IRequestHandler<GetPromoCodesListQuery, List<PromoCodeDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetPromoCodesListQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<PromoCodeDto>> Handle(GetPromoCodesListQuery request, CancellationToken cancellationToken)
        {
            var codes = await _unitOfWork.Repository<PromoCode>().GetAllAsync();

            return codes.Select(c => new PromoCodeDto
            {
                Id = c.Id,
                Code = c.Code,
                DiscountPercent = c.DiscountPercent,
                UsageLimit = c.UsageLimit,
                UsedCount = c.UsedCount
            }).ToList();
        }
    }
}
