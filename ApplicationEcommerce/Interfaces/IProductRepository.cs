using Domain_Ecommerce.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Domain_Ecommerce.Enums;

namespace ApplicationEcommerce.Interfaces
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<(IReadOnlyList<Product> Items, int TotalCount)> GetFilteredAsync(
            Guid? categoryId,
            Gender? gender,
            decimal? minPrice,
            decimal? maxPrice,
            string? sortBy,
            int pageNumber,
            int pageSize);

        Task<IReadOnlyList<Product>> GetBestSellersAsync(int count);
        Task<IReadOnlyList<Product>> GetNewArrivalsAsync(int count);
        Task<Product?> GetWithVariantsAsync(Guid id);
    }
}
