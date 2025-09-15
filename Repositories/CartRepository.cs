using E_CommerceApp.Models;
using Microsoft.EntityFrameworkCore;

namespace E_CommerceApp.Repositories
{
    public class CartRepository : Repository<Cart>, ICartRepository
    {
        public CartRepository(ApplicationDbContext context) : base(context) { }

        public async Task<Cart> GetCartAndCartProductsAsync(string userId) { 
            var cart = await _dbSet
                .Include(cart => cart.CartProducts)
                .ThenInclude(cp => cp.Product)
                .FirstOrDefaultAsync(cart => cart.UserId == userId);
            return cart;
        }
        //It is better to use a CartDTO to load the needed columns only
        
    }
}
