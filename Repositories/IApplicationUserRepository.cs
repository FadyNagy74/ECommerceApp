using E_CommerceApp.Models;

namespace E_CommerceApp.Repositories
{
    public interface IApplicationUserRepository
    {
        public Task<Cart> FindUserCart(string userId);

    }
}
