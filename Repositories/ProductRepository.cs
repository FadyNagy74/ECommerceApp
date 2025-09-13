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

            if (pageNumber < 1) pageNumber = 1;

            var products = await _dbSet.AsNoTracking()
                .Include(product => product.Reviews)
                .Where(product => product.ProductCategories.Any(pc => pc.Category.Name == categoryName))
                .OrderBy(product => product.Name)
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

            if (pageNumber < 1) pageNumber = 1;

            var products = await _dbSet.AsNoTracking()
                .Include(product => product.Reviews)
                .Where(product => product.Price >= start && product.Price <= end)
                .OrderBy(product => product.Name)
                .Skip((pageNumber - 1) * 10)
                .Take(10)
                .ToListAsync();
            return products;
            //If product has no reviews it will return an empty list and not null
        }

        public async Task<List<Product>> GetProductsInCategoryAndPriceRange(string categoryName, decimal minPrice, decimal maxPrice, int pageNumber) {

            if (pageNumber < 1) pageNumber = 1;

            var products = await _dbSet.AsNoTracking()
                .Include(product => product.Reviews)
                .Where(product => product.Price >= minPrice && product.Price <= maxPrice)
                .Where(product => product.ProductCategories.Any(pc => pc.Category.Name == categoryName))
                .OrderBy(product => product.Name)
                .Skip((pageNumber - 1) * 10)
                .Take(10)
                .ToListAsync();
            return products;
        }
        //Putting two wheres only makes it more readable SQL Server still chooes the best execution plan
        //your ordering won't matter


        public async Task<List<Product>> Search(bool filtered, string query, int pageNumber)
        {
            if (pageNumber < 1) pageNumber = 1;

            if (!filtered)
            {
                var products = await _dbSet
                .AsNoTracking()
                .Include(p => p.Reviews)
                .Where(p =>
                    p.Name.Contains(query) ||
                    p.Description.Contains(query) ||
                    p.ProductTags.Any(pt => pt.Tag.Name.Contains(query)))
                .OrderBy(p => p.Name)
                .Skip((pageNumber - 1) * 10)
                .Take(10)
                .ToListAsync();

                return products;
            }
            else
            {
                var products = await _dbSet
                    .AsNoTracking()
                    .Include(p => p.Reviews)
                    .Include(p => p.ProductCategories)
                        .ThenInclude(pc => pc.Category)
                    .Where(p =>
                        p.Name.Contains(query) ||
                        p.Description.Contains(query) ||
                        p.ProductTags.Any(pt => pt.Tag.Name.Contains(query)))
                    .OrderBy(p => p.Name)
                    .Skip((pageNumber - 1) * 10)
                    .Take(10)
                    .ToListAsync();

                return products;
            }
        }





    }
}
