using E_CommerceApp.DTOs;
using E_CommerceApp.Models;

namespace E_CommerceApp.Repositories
{
    public interface IProductRepository : IRepository<Product>
    {
        public Task<List<Product>> GetProductsIncategory(string categoryName, int pageNumber);
        public Task<List<Product>> GetProductsInPriceRange(decimal start, decimal end, int pageNumber);

        public Task<List<Product>> GetProductsInCategoryAndPriceRange(string categoryName, decimal minPrice, decimal maxPrice, int pageNumber);
        public Task<List<Product>> Search(bool filtered, string query, int pageNumber);
        

    }
}
