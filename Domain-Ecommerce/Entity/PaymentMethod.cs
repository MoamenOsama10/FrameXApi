using Domain_Ecommerce.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain_Ecommerce.Entity
{
    public class PaymentMethod : BaseEntity
    {
        public string UserId { get; set; } = string.Empty;

        public string CardType { get; set; } = string.Empty; // Visa / MasterCard
        public string LastFourDigits { get; set; } = string.Empty;
        public string ExpiryDate { get; set; } = string.Empty;

        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
