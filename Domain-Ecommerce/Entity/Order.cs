using Domain_Ecommerce.Common;
using Domain_Ecommerce.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Domain_Ecommerce.Entity
{
    public class Order : BaseEntity
    {
        public string UserId { get; set; } = string.Empty;
        public string OrderNumber { get; set; } = string.Empty;

        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public string? DepositAccount { get; set; }
        public string? DepositReceiptUrl { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Shipping { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }
public bool IsDeleted { get; set; }
       public Guid? ShippingAddressId { get; set; }
public Address? ShippingAddress { get; set; }

        public Guid? PaymentMethodId { get; set; }
        public PaymentMethod? PaymentMethod { get; set; }
public string CustomerName { get; set; } = string.Empty;
public string CustomerEmail { get; set; } = string.Empty;
public string CustomerPhone { get; set; } = string.Empty;
public string City { get; set; } = string.Empty;
public string AddressDetails { get; set; } = string.Empty;
public string? Notes { get; set; }
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
}
