using System.ComponentModel.DataAnnotations.Schema;

namespace E_CommerceApp.Models
{
    [Table("Tags")]
    public class Tag
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public ICollection<ProductTag> ProductTags { get; set; } = new List<ProductTag>();
        public Tag() { 
            Id = Guid.NewGuid().ToString();
        }
    }
}
