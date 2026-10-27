using Domain_Ecommerce.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain_Ecommerce.Entity
{

    public class VariantImage : BaseEntity
    {
        public Guid ProductVariantId { get; set; }
        public ProductVariant ProductVariant { get; set; } = null!;

        public string ImageUrl { get; set; } = string.Empty;
        public bool IsMain { get; set; } = false;
    }
}
