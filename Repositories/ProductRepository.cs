using E_CommerceApp.Models;

namespace E_CommerceApp.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext context) : base(context) { 
        } 

        
    }
}
