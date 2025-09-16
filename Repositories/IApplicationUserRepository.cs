using E_CommerceApp.Models;

namespace E_CommerceApp.Repositories
{
    public interface IApplicationUserRepository : IRepository<ApplicationUser>
    {
        public Task<ApplicationUser> FindUserByIdAsync(string userId);
        public Task<List<Order>> FindUserOrders(string userId);

    }
}
