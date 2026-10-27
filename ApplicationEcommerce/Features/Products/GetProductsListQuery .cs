using ApplicationEcommerce.Common.Models;
using ApplicationEcommerce.DTOs;
using Domain_Ecommerce.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationEcommerce.Features.Products
{
    public class GetProductsListQuery : IRequest<PagedResult<ProductDto>>
    {
        public Guid? CategoryId { get; set; }
        public Gender? Gender { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? SortBy { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 12;
    }
}
