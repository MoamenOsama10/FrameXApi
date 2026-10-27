using ApplicationEcommerce.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationEcommerce.Features.Order
{

    public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderDto?>
    {
        private readonly IOrderRepository _orderRepository;

        public GetOrderByIdQueryHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<OrderDto?> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetWithItemsAsync(request.Id);

            if (order is null) return null;

            return new OrderDto
            {
                Id = order.Id,
                OrderNumber = order.OrderNumber,
                Status = order.Status.ToString(),
                Total = order.Total,
                CreatedAt = order.CreatedAt,
                DepositAccount = order.DepositAccount,
                DepositReceiptUrl = order.DepositReceiptUrl,
                Items = order.Items.Select(i => new OrderItemDto
                {
                    ProductName = i.ProductVariant.Product.Name,
                    Color = i.ProductVariant.Color,
                    Size = i.ProductVariant.Size.ToString(),
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    ImageUrl = i.ProductVariant.Images.FirstOrDefault(img => img.IsMain)?.ImageUrl
                }).ToList()
            };
        }
    }
}
