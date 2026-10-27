using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationEcommerce.DTOs
{
    public class VariantImageDto
    {
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsMain { get; set; }
    }
}
