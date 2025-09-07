using E_CommerceApp.Models;
using E_CommerceApp.Repositories;

namespace E_CommerceApp.Services
{
    public class CategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        public CategoryService(ICategoryRepository categoryRepository) { 
            _categoryRepository = categoryRepository;
        }

        public async Task<int> Add(string categoryName) {
            bool exists = await _categoryRepository.CategoryExistsAsync(categoryName);
            if (exists) {
                return -1;  //-1 means category exists
            }
            Category? category = new Category();
            category.Name = categoryName;
            _categoryRepository.Add(category);
            int result = await _categoryRepository.SaveChangesAsync();
            return result;
        }

        public async Task<Category?> FindByName(string categoryName) { 
            Category? category = await _categoryRepository.FindUsingNameAsync(categoryName);
            if (category == null)
            {
                return null;
            }
            return category;
        }

        public async Task<int> Remove(Category category) {
            _categoryRepository.Remove(category);
            int result = await _categoryRepository.SaveChangesAsync();
            return result;
        }
    }
}
