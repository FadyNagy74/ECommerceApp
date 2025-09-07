using E_CommerceApp.Models;
using Microsoft.EntityFrameworkCore;

namespace E_CommerceApp.Repositories
{
    public class TagRepository : Repository<Tag>, ITagRepositry
    {
        public TagRepository(ApplicationDbContext context) : base(context) { 
        }

        public async Task<bool> ExistsAsync(string tagName)
        {
            bool exists = await _dbSet.AnyAsync(tag => tag.Name == tagName);
            return exists;
        }

        public async Task<Tag?> FindUsingNameAsync(string tagName)
        {
            Tag? tag = await _dbSet.FirstOrDefaultAsync(tag => tag.Name == tagName);
            if (tag == null) return null;
            return tag;
        }

        public async Task<List<Tag>> FindUsingNamesAsync(List<string> tagNames)
        {
            List<Tag> tags = await _dbSet.Where(tag => tagNames.Contains(tag.Name)).ToListAsync();
            /*
             * SELECT [t].[Id], [t].[Name]
             * FROM [Tags] AS [c]
             * WHERE [t].[Name] IN (N'Electronics', N'Clothing', N'Books')
             * SQL equivalent to above code
             */
            return tags;
        }
    }
}
