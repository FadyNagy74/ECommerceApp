using E_CommerceApp.Models;

namespace E_CommerceApp.Repositories
{
    public interface ITagRepositry : IRepository<Tag>
    {
        public Task<bool> ExistsAsync(string tagName);
        public Task<Tag?> FindUsingNameAsync(string tagName);

        public Task<List<Tag>> FindUsingNamesAsync(List<string> tagNames);
    }
}
