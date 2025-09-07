using E_CommerceApp.Models;

namespace E_CommerceApp.Repositories
{
    public interface ICategoryRepository : IRepository<Category>
    {
        public Task<bool> CategoryExistsAsync(string categoryName);
        public Task<Category?> FindUsingNameAsync(string categoryName);

        public Task<List<Category>> FindUsingNamesAsync(List<string> categoryNames);
    }
}
