using ApplicationEcommerce.Interfaces;
using MediatR;

namespace ApplicationEcommerce.Features.Order
{
    public class DeleteOrderCommandHandler : IRequestHandler<DeleteOrderCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteOrderCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(
            DeleteOrderCommand request,
            CancellationToken cancellationToken)
        {
            var orderRepository =
                _unitOfWork.Repository<Domain_Ecommerce.Entity.Order>();

            var order = await orderRepository.GetByIdAsync(request.Id);

            if (order == null)
                return;

            // Soft delete
            order.IsDeleted = true;

            await _unitOfWork.SaveChangesAsync();
        }
    }
}