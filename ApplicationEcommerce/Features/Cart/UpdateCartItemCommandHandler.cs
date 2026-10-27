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
    public class UpdateCartItemCommandHandler : IRequestHandler<UpdateCartItemCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateCartItemCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(UpdateCartItemCommand request, CancellationToken cancellationToken)
        {
            var item = await _unitOfWork.Repository<CartItem>().GetByIdAsync(request.CartItemId)
                ?? throw new NotFoundException(nameof(CartItem), request.CartItemId);

            item.Quantity = request.Quantity;
            _unitOfWork.Repository<CartItem>().Update(item);

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
