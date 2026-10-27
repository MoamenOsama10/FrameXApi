using ApplicationEcommerce.Common.Exceptions;
using ApplicationEcommerce.Interfaces;
using Domain_Ecommerce.Entity;
using Domain_Ecommerce.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationEcommerce.Common.Checkout
{
    public class CheckoutCommandHandler : IRequestHandler<CheckoutCommand, Guid>
    {
        private readonly ICartRepository _cartRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CheckoutCommandHandler(
            ICartRepository cartRepository,
            IOrderRepository orderRepository,
            IUnitOfWork unitOfWork)
        {
            _cartRepository = cartRepository;
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(CheckoutCommand request, CancellationToken cancellationToken)
        {
            var cart = await _cartRepository.GetByUserIdAsync(request.UserId);

            if (cart is null || !cart.Items.Any())
                throw new BadRequestException("Cart is empty.");

            // تحقق من الـ Stock قبل ما نكمل
            foreach (var item in cart.Items)
            {
                if (item.ProductVariant.StockQuantity < item.Quantity)
                    throw new BadRequestException(
                        $"Insufficient stock for {item.ProductVariant.Product.Name}.");
            }

            var subtotal = cart.Items.Sum(i => i.ProductVariant.Price * i.Quantity);
            var shipping = subtotal >= 50 ? 0 : 10; // Free shipping over $50
            var tax = Math.Round(subtotal * 0.05m, 2); // مثال: 5% ضريبة
            var total = subtotal + shipping + tax;

            var order = new Order
            {
                UserId = request.UserId,
                OrderNumber = GenerateOrderNumber(),
                Status = OrderStatus.Pending,
                Subtotal = subtotal,
                Shipping = shipping,
                Tax = tax,
                Total = total,
                ShippingAddressId = request.ShippingAddressId,
                PaymentMethodId = request.PaymentMethodId,
                DepositAccount = request.DepositAccount,
                DepositReceiptUrl = request.DepositReceiptUrl,
                Items = cart.Items.Select(i => new OrderItem
                {
                    ProductVariantId = i.ProductVariantId,
                    Quantity = i.Quantity,
                    UnitPrice = i.ProductVariant.Price
                }).ToList()
            };

            // خصم الكمية من الـ Stock
            foreach (var item in cart.Items)
            {
                item.ProductVariant.StockQuantity -= item.Quantity;
            }

            await _orderRepository.AddAsync(order);

            // تفريغ الكارت بعد إنشاء الأوردر
            cart.Items.Clear();

            await _unitOfWork.SaveChangesAsync();

            return order.Id;
        }

        private static string GenerateOrderNumber()
            => $"FX{DateTime.UtcNow:yyMMdd}{Random.Shared.Next(1000, 9999)}";
    }
}
