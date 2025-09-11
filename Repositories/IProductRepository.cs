using E_CommerceApp.DTOs;
using E_CommerceApp.Models;

namespace E_CommerceApp.Repositories
{
    public interface IProductRepository : IRepository<Product>
    {
        public Task<List<Product>> GetProductsIncategory(string categoryName, int pageNumber);
        public Task<List<Product>> GetProductsInPriceRange(decimal start, decimal end, int pageNumber);
        public Task<List<Product>> GetProductsSortedAsc(int pageNumber);
        public Task<List<Product>> GetProductsSortedDsc(int pageNumber);
        public Task<List<ViewProductWithRateDTO>> GetProductsSortedReviewAsc(int pageNumber);
        public Task<List<ViewProductWithRateDTO>> GetProductsSortedReviewDsc(int pageNumber);
        public Task<List<Product>> Search(string query);
        

    }
}
