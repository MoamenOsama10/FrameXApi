using ApplicationEcommerce.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationEcommerce.Features.Order
{
    public class GetOrdersListQueryHandler : IRequestHandler<GetOrdersListQuery, List<OrderDto>>
    {
        private readonly IOrderRepository _orderRepository;

        public GetOrdersListQueryHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<List<OrderDto>> Handle(GetOrdersListQuery request, CancellationToken cancellationToken)
        {
            var orders = string.IsNullOrEmpty(request.UserId)
                ? await _orderRepository.GetAllOrdersAsync()
                : await _orderRepository.GetByUserIdAsync(request.UserId);

            return orders.Select(o => new OrderDto
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                Status = o.Status.ToString(),
                Subtotal = o.Subtotal,
                Shipping = o.Shipping,
                Total = o.Total,
                CreatedAt = o.CreatedAt,
                CustomerName = o.CustomerName,
                CustomerEmail = o.CustomerEmail,
                CustomerPhone = o.CustomerPhone,
                City = o.City,
                AddressDetails = o.AddressDetails,
                Notes = o.Notes,
                DepositAccount = o.DepositAccount,
                DepositReceiptUrl = o.DepositReceiptUrl,
                Items = o.Items.Select(i => new OrderItemDto
                {
                    ProductName = i.ProductVariant.Product.Name,
                    Color = i.ProductVariant.Color,
                    Size = i.ProductVariant.Size.ToString(),
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    CustomImageUrl = i.CustomImageUrl,
                    ImageUrl = i.ProductVariant.Images.FirstOrDefault(img => img.IsMain)?.ImageUrl
                        ?? i.ProductVariant.Images.FirstOrDefault()?.ImageUrl
                        ?? i.ProductVariant.Product.ImageUrl
                }).ToList()
            }).ToList();
        }
    }
}