using E_CommerceApp.Models;
using E_CommerceApp.Repositories;

namespace E_CommerceApp.Services
{
    public class ProductService
    {
        private readonly IProductRepository _productRepository;
        public ProductService(IProductRepository productRepository) { 
            _productRepository = productRepository;
        }

        public async Task<int> AddProduct(Product product) { 
            _productRepository.Add(product);
            int result = await _productRepository.SaveChangesAsync();
            return result;
        }

        public async Task<int> RemoveProduct(Product product) { 
            _productRepository.Remove(product);
            int result = await _productRepository.SaveChangesAsync();
            return result;
        }

        public async Task<Product> GetById(string productId) { 
            Product? product = await _productRepository.GetByIdAsync(productId);
            if (product == null) return null;
            return product;
        }
    }
}
