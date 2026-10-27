using Domain_Ecommerce.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain_Ecommerce.Entity
{
    public class Address : BaseEntity
    {
        public string UserId { get; set; } = string.Empty;

        public string Label { get; set; } = string.Empty; // Home / Work
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public bool IsDefault { get; set; } = false;

        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
