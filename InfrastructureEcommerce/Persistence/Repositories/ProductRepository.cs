using ApplicationEcommerce.Interfaces;
using Domain_Ecommerce.Entity;
using Domain_Ecommerce.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureEcommerce.Persistence.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<(IReadOnlyList<Product> Items, int TotalCount)> GetFilteredAsync(
            Guid? categoryId,
            Gender? gender,
            decimal? minPrice,
            decimal? maxPrice,
            string? sortBy,
            int pageNumber,
            int pageSize)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Variants)
                    .ThenInclude(v => v.Images)
                .AsQueryable();

            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId.Value);

            if (gender.HasValue)
                query = query.Where(p => p.Gender == gender.Value);

            if (minPrice.HasValue)
                query = query.Where(p => p.BasePrice >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(p => p.BasePrice <= maxPrice.Value);

            query = sortBy switch
            {
                "price_asc" => query.OrderBy(p => p.BasePrice),
                "price_desc" => query.OrderByDescending(p => p.BasePrice),
                "rating" => query.OrderByDescending(p => p.Rating),
                "newest" => query.OrderByDescending(p => p.CreatedAt),
                _ => query.OrderByDescending(p => p.CreatedAt) // Featured/Default
            };

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<IReadOnlyList<Product>> GetBestSellersAsync(int count)
        {
            // مبني على عدد مرات ظهور الـ Variant بتاعه في OrderItems
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Variants).ThenInclude(v => v.Images)
                .OrderByDescending(p => p.Variants.SelectMany(v => v.OrderItems).Sum(oi => oi.Quantity))
                .Take(count)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Product>> GetNewArrivalsAsync(int count)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Variants).ThenInclude(v => v.Images)
                .OrderByDescending(p => p.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<Product?> GetWithVariantsAsync(Guid id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Variants).ThenInclude(v => v.Images)
                .Include(p => p.Reviews)
                .FirstOrDefaultAsync(p => p.Id == id);
        }
    }
}
