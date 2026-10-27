using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationEcommerce.DTOs
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public decimal BasePrice { get; set; }
        public decimal Rating { get; set; }
        public int ReviewsCount { get; set; }
        public bool IsNew { get; set; }
        public bool IsOnSale { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsCustomizable { get; set; }
        public List<ProductVariantDto> Variants { get; set; } = new();
    }
}
