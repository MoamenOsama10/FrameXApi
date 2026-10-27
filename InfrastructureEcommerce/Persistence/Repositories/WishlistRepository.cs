using ApplicationEcommerce.Interfaces;
using Domain_Ecommerce.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureEcommerce.Persistence.Repositories
{
    public class WishlistRepository : GenericRepository<Wishlist>, IWishlistRepository
    {
        private readonly ApplicationDbContext _context;

        public WishlistRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Wishlist?> GetByUserIdAsync(string userId)
        {
            return await _context.Wishlists
                .Include(w => w.Items)
                    .ThenInclude(i => i.Product)
                        .ThenInclude(p => p.Variants)
                            .ThenInclude(v => v.Images)
                .FirstOrDefaultAsync(w => w.UserId == userId);
        }
    }
}
