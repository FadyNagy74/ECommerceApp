using E_CommerceApp.Models;
using Microsoft.EntityFrameworkCore;

namespace E_CommerceApp.Repositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<bool> CategoryExistsAsync(string categoryName) {
            bool exists = await _dbSet.AnyAsync(category => category.Name == categoryName);
            return exists;
        }

        public async Task<Category?> FindUsingNameAsync(string categoryName) { 
            Category? category = await _dbSet.FirstOrDefaultAsync(category => category.Name == categoryName);
            if (category == null) return null;
            return category;
        }

        public async Task<List<Category>> FindUsingNamesAsync(List<string> categoryNames) {
            List<Category> categories = await _dbSet.Where(category => categoryNames.Contains(category.Name)).ToListAsync();
            /*
             * SELECT [c].[Id], [c].[Name]
             * FROM [Categories] AS [c]
             * WHERE [c].[Name] IN (N'Electronics', N'Clothing', N'Books')
             * SQL equivalent to above code
             */
            return categories;
        }
        
    }
}
