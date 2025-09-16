using E_CommerceApp.Models;
using Microsoft.EntityFrameworkCore;

namespace E_CommerceApp.Repositories
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        public OrderRepository(ApplicationDbContext context) : base(context) { }

        public async Task<Order?> GetOrderWithItems(string orderId)
        {
            Order? order = await _dbSet.Include(order => order.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(order => order.Id == orderId);
            return order;
        }

        public async Task<List<Order>> GetOrdersStatusSorted(bool asc, int pageNumber)
        {
            if (pageNumber < 1) { pageNumber = 1; }
            List<Order> orders;
            if (asc)
            {
                orders = await _dbSet.AsNoTracking()
                    .OrderBy(order => order.Status)
                    .Skip((pageNumber - 1) * 10)
                    .Take(10)
                    .ToListAsync();
            }
            else
            {
                orders = await _dbSet.AsNoTracking()
                    .OrderByDescending(order => order.Status)
                    .Skip((pageNumber - 1) * 10)
                    .Take(10)
                    .ToListAsync();
            }

            return orders;
        }

        public async Task<List<Order>> GetOrdersDateSorted(bool asc, int pageNumber)
        {
            if (pageNumber < 1) { pageNumber = 1; }
            List<Order> orders;
            if (asc)
            {
                orders = await _dbSet.AsNoTracking()
                    .OrderBy(order => order.PlacedAt)
                    .Skip((pageNumber - 1) * 10)
                    .Take(10)
                    .ToListAsync();
            }
            else
            {
                orders = await _dbSet.AsNoTracking()
                    .OrderByDescending(order => order.PlacedAt)
                    .Skip((pageNumber - 1) * 10)
                    .Take(10)
                    .ToListAsync();
            }

            return orders;
        }
    }
}
