using E_CommerceApp.Models;

namespace E_CommerceApp.Repositories
{
    public interface ICartRepository : IRepository<Cart>
    {
        public Task<Cart> GetCartAndCartProductsAsync(string userId);
    }
}
