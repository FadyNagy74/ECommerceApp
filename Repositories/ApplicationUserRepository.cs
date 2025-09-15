using E_CommerceApp.Models;
using Microsoft.EntityFrameworkCore;

namespace E_CommerceApp.Repositories
{
    public class ApplicationUserRepository : IApplicationUserRepository
    {
        private readonly ApplicationDbContext _context;

        public ApplicationUserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Cart> FindUserCart(string userId) {
           var user =  await _context.Users.Include(user => user.Cart).FirstOrDefaultAsync(user => user.Id == userId);
            return user.Cart;
        }
    }
}
