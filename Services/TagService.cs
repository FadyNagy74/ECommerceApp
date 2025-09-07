using E_CommerceApp.Models;
using E_CommerceApp.Repositories;

namespace E_CommerceApp.Services
{
    public class TagService
    {
        private readonly ITagRepositry _tagRepository;

        public TagService(ITagRepositry tagRepository)
        {
            _tagRepository = tagRepository;
        }

        public async Task<int> Add(string tagName)
        {
            bool exists = await _tagRepository.ExistsAsync(tagName);
            if (exists)
            {
                return -1;  //-1 means tag exists
            }
            Tag? tag = new Tag();
            tag.Name = tagName;
            _tagRepository.Add(tag);
            int result = await _tagRepository.SaveChangesAsync();
            return result;
        }

        public async Task<Tag?> FindByName(string tagName)
        {
            Tag? tag = await _tagRepository.FindUsingNameAsync(tagName);
            if (tag == null)
            {
                return null;
            }
            return tag;
        }

        public async Task<int> Remove(Tag tag)
        {
            _tagRepository.Remove(tag);
            int result = await _tagRepository.SaveChangesAsync();
            return result;
        }
    }
}
