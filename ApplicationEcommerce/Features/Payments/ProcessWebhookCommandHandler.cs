using ApplicationEcommerce.Common.Exceptions;
using ApplicationEcommerce.Interfaces;
using Domain_Ecommerce.Enums;
using MediatR;
using Microsoft.Extensions.Logging;
using OrderEntity = Domain_Ecommerce.Entity.Order;

namespace ApplicationEcommerce.Features.Payments
{
    public class ProcessWebhookCommandHandler : IRequestHandler<ProcessWebhookCommand>
    {
        private readonly IPaymentService _paymentService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ProcessWebhookCommandHandler> _logger;

        public ProcessWebhookCommandHandler(
            IPaymentService paymentService,
            IUnitOfWork unitOfWork,
            ILogger<ProcessWebhookCommandHandler> logger)
        {
            _paymentService = paymentService;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Handle(ProcessWebhookCommand request, CancellationToken cancellationToken)
        {
            if (!_paymentService.VerifyWebhookSignature(request.Hmac, request.Data))
                throw new BadRequestException("Invalid webhook signature.");

            if (!Guid.TryParse(request.MerchantOrderId, out var orderId))
                return;

            var order = await _unitOfWork.Repository<OrderEntity>().GetByIdAsync(orderId);
            if (order is null)
                return;

            // الأوردر "لسه متدفعش" لو كان Pending أو اتلغى بسبب محاولة فاشلة قبل كده
            var notPaidYet = order.Status == OrderStatus.Pending || order.Status == OrderStatus.Cancelled;

            if (request.Success)
            {
                // إشعار متكرر لأوردر اتدفع خلاص: نتجاهله
                if (!notPaidYet)
                    return;

                // المبلغ والعملة لازم يطابقوا الأوردر بالظبط
                var expectedCents = (long)Math.Round(order.Total * 100m, MidpointRounding.AwayFromZero);

                long paidCents = 0;
                var amountOk = request.Data.TryGetValue("amount_cents", out var centsText)
                    && long.TryParse(centsText, out paidCents)
                    && paidCents == expectedCents;

                var currencyOk = request.Data.TryGetValue("currency", out var currency) && currency == "EGP";

                if (!amountOk || !currencyOk)
                {
                    _logger.LogError(
                        "Payment webhook mismatch for order {OrderNumber}: expected {Expected} cents EGP, got {Paid} cents {Currency}",
                        order.OrderNumber, expectedCents, paidCents, currency);
                    return; // نرجّع 200 عشان Paymob ميكرّرش، والأوردر يفضل من غير ما يتعلّم مدفوع
                }

                order.Status = OrderStatus.Processing;
            }
            else
            {
                // الفشل بيلغي الأوردر بس لو لسه Pending، ومش بيلمس أوردر اتدفع
                if (order.Status != OrderStatus.Pending)
                    return;

                order.Status = OrderStatus.Cancelled;
            }

            _unitOfWork.Repository<OrderEntity>().Update(order);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}