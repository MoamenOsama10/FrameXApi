using Domain_Ecommerce.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain_Ecommerce.Entity
{
    public class Wishlist : BaseEntity
    {
        public string UserId { get; set; } = string.Empty;

        public ICollection<WishlistItem> Items { get; set; } = new List<WishlistItem>();
    }
}
