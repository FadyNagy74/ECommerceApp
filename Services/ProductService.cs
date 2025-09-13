using E_CommerceApp.DTOs;
using E_CommerceApp.Models;
using E_CommerceApp.Repositories;
using Microsoft.IdentityModel.Tokens;

namespace E_CommerceApp.Services
{
    public class ProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ITagRepositry _tagRepositry;
        private readonly IReviewRepository _reviewRepository;
        public ProductService(IProductRepository productRepository, ICategoryRepository categoryRepository, ITagRepositry tagRepositry, IReviewRepository reviewRepository) { 
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _tagRepositry = tagRepositry;
            _reviewRepository = reviewRepository;
        }

        public async Task<int> AddProduct(ProductDTO productDTO) {

            if (productDTO.Price < 0) throw new ArgumentOutOfRangeException("Price must be a positive number");
            if (productDTO.Stock < 0) throw new ArgumentOutOfRangeException("Stock must be a positive number"); 

            Product product = new Product();
            product.Name = productDTO.Name;
            product.Description = productDTO.Description;
            product.Price = productDTO.Price;
            product.Stock = productDTO.Stock;

            List<ProductCategory> productCategories = new List<ProductCategory>();
            List<ProductTag> productTags = new List<ProductTag>();

            if (productDTO.Categories.Count == 0) { 
                throw new ArgumentException("You must enter a category");
            }

            if (productDTO.Tags.Count == 0) {
                throw new ArgumentException("You must enter a tag");
            }

            List<Category> foundCategories = await _categoryRepository.FindUsingNamesAsync(productDTO.Categories);
            if (foundCategories.Count == 0) throw new KeyNotFoundException("No Categories with these names were found");

            List<Tag> foundTags = await _tagRepositry.FindUsingNamesAsync(productDTO.Tags);
            if (foundTags.Count == 0) throw new KeyNotFoundException("No Tags with these names were found");

            var foundCategoryNames = foundCategories.Select(c => c.Name).ToList();
            //Here we select names from categories (all in memory no db queries needed)
            var foundTagNames = foundTags.Select(t => t.Name).ToList();

            var missingCategories = productDTO.Categories.Except(foundCategoryNames).ToList();
            //Here we use Except to find names that are in productDTO (given that are not in our found names)
            var missingTags = productDTO.Tags.Except(foundTagNames).ToList();

            bool differenceInCategories = missingCategories.Any();
            //Here we see whether anything is found inside the missing list (could just check for count)
            bool differenceInTags = missingTags.Any();

            if (differenceInCategories) throw new KeyNotFoundException($"Categories not found: {String.Join(",",missingCategories)}");
            if (differenceInTags) throw new KeyNotFoundException($"Tags not found: {String.Join(",", missingTags)}");

            foreach (var category in foundCategories) {
                ProductCategory? productCategory = new ProductCategory();
                productCategory.Product = product;
                productCategory.Category = category;
                productCategories.Add(productCategory);
            }

            foreach (var tag in foundTags)
            {
                ProductTag? productTag = new ProductTag();
                productTag.Tag = tag;
                productTag.Product = product;
                productTags.Add(productTag);
            }
            product.ProductCategories = productCategories;
            product.ProductTags = productTags;
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
            if (product == null) throw new KeyNotFoundException("Product with this Id was not found");
            return product;
        }

        
        public async Task<List<ShowReviewDTO>?> GetProductReviews(string productId)
        {
            var reviews = await _reviewRepository.GetProductReviews(productId);
            if (reviews == null) return null;
            return reviews;
        }

        public async Task<List<Product>> GetProductsInCategory(string categoryName, int pageNumber) { 

            var products = await _productRepository.GetProductsIncategory(categoryName, pageNumber);
            if(!products.Any()) throw new KeyNotFoundException($"No products found in category '{categoryName} or category doesn't exist'");
            return products;

        }

        public async Task<List<Product>> GetProducsInPriceRange(decimal start, decimal end, int pageNumber) {
            var products = await _productRepository.GetProductsInPriceRange(start, end, pageNumber);
            if (!products.Any()) throw new KeyNotFoundException($"No products were found in this price range");
            return products;
        }


        public async Task<List<Product>> GetProductsInCategoryAndPriceRange(string categoryName, decimal minPrice, decimal maxPrice,int pageNumber) {
            List<Product> filteredProducts = new List<Product>();
            filteredProducts = await _productRepository.GetProductsInCategoryAndPriceRange(categoryName, minPrice, maxPrice, pageNumber);
            if(!filteredProducts.Any()) throw new KeyNotFoundException($"No products were found");
            return filteredProducts;
        }

        public List<Product> SortProductsPrice(List<Product> products, bool asc) {
            //No need to check for null it will be made sure that it isn't null from caller
            List<Product> sortedProducts = new List<Product>();
            if (asc) { sortedProducts = products.OrderBy(product => product.Price).ToList(); }
            else { sortedProducts = products.OrderByDescending(product => product.Price).ToList(); }

            //OrderBy returns IOrderedEnumerable
            //IOrderedEnumerable<T> is a sorting recipe that re-applies each time you iterate;
            //use .ToList() to freeze the sorted results
            //for example when we add elements to our existing IOrderedEnumerable when we iterate we sort again

            return sortedProducts;
        }

        public List<Product> SortProductsReviews(List<Product> products, bool asc) {

            List<Product> sortedProducts = new List<Product>();

            if (asc) { sortedProducts = products.OrderBy(product => product.Reviews.Any() ? product.Reviews.Average(review => review.RateValue) : 0).ToList(); }
            else { sortedProducts = products.OrderByDescending(product => product.Reviews.Any() ? product.Reviews.Average(review => review.RateValue) : 0).ToList(); }

            return sortedProducts;
        }

        public async Task<List<Product>> GetAllProducts() {
            var products = await _productRepository.GetAllAsync();
            if(!products.Any()) throw new KeyNotFoundException("No products were found");
            return products;
        }

        public async Task<List<Product>> SearchProducts(bool filtered, string query, int pageNumber) {
            var products = await _productRepository.Search(filtered ,query, pageNumber);
            if (!products.Any()) throw new KeyNotFoundException("No products were found");

            return products;
        }


    }
}
