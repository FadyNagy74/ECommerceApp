using E_CommerceApp.Models;

namespace E_CommerceApp.Repositories
{
    public interface ICartProductRepository : IRepository<CartProduct>
    {
        public Task<CartProduct> GetCartProductAsync(string productId, string cartId);
    }
}
