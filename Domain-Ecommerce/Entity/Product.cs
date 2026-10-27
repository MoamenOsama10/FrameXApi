using Domain_Ecommerce.Common;
using Domain_Ecommerce.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain_Ecommerce.Entity
{
    public class Product : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public Guid CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        public Gender Gender { get; set; }

        public decimal BasePrice { get; set; }

        public decimal Rating { get; set; } = 0;
        public int ReviewsCount { get; set; } = 0;
        public bool IsCustomizable { get; set; }
public bool IsDeleted { get; set; }
        public bool IsNew { get; set; } = false;
        public bool IsOnSale { get; set; } = false;
        public string? ImageUrl { get; set; }
        public ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
