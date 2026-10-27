using ApplicationEcommerce.Common.Exceptions;
using ApplicationEcommerce.Interfaces;
using Domain_Ecommerce.Entity;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationEcommerce.Features.Cart
{
    public class RemoveCartItemCommandHandler : IRequestHandler<RemoveCartItemCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public RemoveCartItemCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(RemoveCartItemCommand request, CancellationToken cancellationToken)
        {
            var item = await _unitOfWork.Repository<CartItem>().GetByIdAsync(request.CartItemId)
                ?? throw new NotFoundException(nameof(CartItem), request.CartItemId);

            _unitOfWork.Repository<CartItem>().Delete(item);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
