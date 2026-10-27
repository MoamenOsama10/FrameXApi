using Domain_Ecommerce.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationEcommerce.Interfaces
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        Task<IReadOnlyList<Order>> GetByUserIdAsync(string userId);
        Task<IReadOnlyList<Order>> GetAllOrdersAsync();
        Task<Order?> GetWithItemsAsync(Guid id);
    }
}