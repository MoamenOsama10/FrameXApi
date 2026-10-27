using ApplicationEcommerce.Common.Exceptions;
using ApplicationEcommerce.Interfaces;
using MediatR;
using OrderEntity = Domain_Ecommerce.Entity.Order;

namespace ApplicationEcommerce.Features.Payments
{
    public class InitiatePaymentCommandHandler : IRequestHandler<InitiatePaymentCommand, string>
    {
        private readonly IPaymentService _paymentService;
        private readonly IUnitOfWork _unitOfWork;

        public InitiatePaymentCommandHandler(IPaymentService paymentService, IUnitOfWork unitOfWork)
        {
            _paymentService = paymentService;
            _unitOfWork = unitOfWork;
        }

        public async Task<string> Handle(InitiatePaymentCommand request, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(request.OrderId, out var orderId))
                throw new BadRequestException("رقم الطلب غير صالح.");

            var order = await _unitOfWork.Repository<OrderEntity>().GetByIdAsync(orderId)
                ?? throw new NotFoundException("Order", request.OrderId);

            // المبلغ والبيانات من الأوردر نفسه، مش من اللي جاي من الموقع
            return await _paymentService.InitiatePaymentAsync(
                order.Total, order.Id.ToString(), order.CustomerEmail, order.CustomerName);
        }
    }
}