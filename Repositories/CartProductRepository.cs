using E_CommerceApp.Models;
using Microsoft.EntityFrameworkCore;

namespace E_CommerceApp.Repositories
{
    public class CartProductRepository : Repository<CartProduct>, ICartProductRepository
    {
        public CartProductRepository(ApplicationDbContext context) : base(context) { }

        public async Task<CartProduct?> GetCartProductAsync(string productId, string cartId) {
            CartProduct? cartProduct = await _dbSet.FirstOrDefaultAsync(cp => cp.CartId == cartId && cp.ProductId == productId);
            return cartProduct;
        }
    }
}
