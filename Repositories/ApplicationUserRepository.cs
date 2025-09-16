using E_CommerceApp.Models;
using Microsoft.EntityFrameworkCore;

namespace E_CommerceApp.Repositories
{
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
    {
        public ApplicationUserRepository(ApplicationDbContext context) : base(context) { }
        public async Task<ApplicationUser?> FindUserByIdAsync(string userId)
        {
            ApplicationUser? user = await _dbSet.AsNoTracking()
                .Include(user => user.UserAddresses)
                .FirstOrDefaultAsync(user => user.Id == userId);
            return user;
        }

        public async Task<List<Order>> FindUserOrders(string userId) 
        {
            var user = await _dbSet
                .AsNoTracking()
                    .Include(u => u.Orders)
                        .ThenInclude(o => o.OrderItems)
                            .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user != null && user.Orders != null)
                return user.Orders.ToList();
            else
                return new List<Order>();
        }
    }
}
