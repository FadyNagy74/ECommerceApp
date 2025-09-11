using E_CommerceApp.DTOs;
using E_CommerceApp.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;

namespace E_CommerceApp.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext context) : base(context) {
        }

        //Note that pagination should be AFTER filtering because if we paginate before we take
        //10 Products and filter on them even though other results might be in he products table
        //We might not return anything

        public async Task<List<Product>> GetProductsIncategory(string categoryName, int pageNumber) {
            var products = await _dbSet.AsNoTracking()
                .Where(product => product.ProductCategories.Any(pc => pc.Category.Name == categoryName))
                .Skip((pageNumber - 1) * 10)
                .Take(10)
                .ToListAsync();
                     
            return products;

        }
        /*
         * SELECT p.*
FROM Products AS p
WHERE EXISTS (
    SELECT 1
    FROM ProductCategories AS pc
    INNER JOIN Categories AS c ON pc.CategoryId = c.Id
    WHERE pc.ProductId = p.Id
      AND c.Name = @__categoryName
);

         */
        public async Task<List<Product>> GetProductsInPriceRange(decimal start, decimal end, int pageNumber) {
            var products = await _dbSet.AsNoTracking()
                .Where(product => product.Price >= start && product.Price <= end)
                .Skip((pageNumber - 1) * 10)
                .Take(10)
                .ToListAsync();
            return products;
        }

        public async Task<List<Product>> GetProductsSortedAsc(int pageNumber) {
            var products = await _dbSet.AsNoTracking()
                .OrderBy(product => product.Price)
                .Skip((pageNumber - 1) * 10)
                .Take(10)
                .ToListAsync();
            return products;
        }

        public async Task<List<Product>> GetProductsSortedDsc(int pageNumber)
        {
            var products = await _dbSet.AsNoTracking()
                .OrderByDescending(product => product.Price)
                .Skip((pageNumber - 1) * 10)
                .Take(10)
                .ToListAsync();
            return products;
        }

        public async Task<List<ViewProductWithRateDTO>> GetProductsSortedReviewAsc(int pageNumber) {
            var products = await _dbSet.AsNoTracking()
                .Select(product => new ViewProductWithRateDTO
                {
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    Stock = product.Stock,
                    ReviewRate = product.Reviews
                        .Select(r => (int?)r.RateValue)
                        .Average() ?? 0
                    //Order by only orders by a single value and not a whole collection
                    //so we can't OrderBy(product => (1) product.Reviews.OrderBy(r => r.RateValue))
                    //Here (1) returns a list of products so we can't OrderBy a list of products

                    //.Select(r => r.RateValue) won't return if something is null and .Average() won't
                    //understand what to return if something is null

                    //We use .Average() so we get the average of all review rates for a product

                })
                .OrderBy(x => x.ReviewRate)
                .Skip((pageNumber - 1) * 10)
                .Take(10)
                .ToListAsync();
            return products;
        }

        public async Task<List<ViewProductWithRateDTO>> GetProductsSortedReviewDsc(int pageNumber)
        {
            var products = await _dbSet.AsNoTracking()
                .Select(product => new ViewProductWithRateDTO
                {
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    Stock = product.Stock,
                    ReviewRate = product.Reviews
                        .Select(r => (int?)r.RateValue)
                        .Average() ?? 0
                    //Order by only orders by a single value and not a whole collection
                    //so we can't OrderBy(product => (1) product.Reviews.OrderBy(r => r.RateValue))
                    //Here (1) returns a list of products so we can't OrderBy a list of products
                })
                .OrderByDescending(x => x.ReviewRate)
                .Skip((pageNumber - 1) * 10)
                .Take(10)
                .ToListAsync();
            return products;
        }

        public async Task<List<Product>> Search(string query) {

            var products = await _dbSet
                .FromSqlInterpolated($"EXEC SearchProducts {query}")
                .AsNoTracking()
                .ToListAsync();

            //We cannot use Select or any Linq related methods with FromSqlInterpolated
            //So we return the products first then we bring them into memory and do the actual selection

            //FromSqlInterprolated performs a stored procedure that returns only names
            //EF Core tries to map other fields to the Product model but they are null
            //So this won't work so we have to select only the name

            //AsNoTracking cannot be used before FromSqlInterpolated

            return products;
        }

        


    }
}
