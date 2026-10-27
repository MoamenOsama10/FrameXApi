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
    public class DeletePromoCodeCommandHandler : IRequestHandler<DeletePromoCodeCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeletePromoCodeCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(DeletePromoCodeCommand request, CancellationToken cancellationToken)
        {
            var promoCode = await _unitOfWork.Repository<PromoCode>().GetByIdAsync(request.Id)
                ?? throw new NotFoundException(nameof(PromoCode), request.Id);

            _unitOfWork.Repository<PromoCode>().Delete(promoCode);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
