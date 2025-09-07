using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_CommerceApp.Models
{
    [Table("Categories")]
    public class Category
    {
        public string Id { get; set; }
        [MinLength(3)]
        public string Name { get; set; }

        public Category() { 
            Id = Guid.NewGuid().ToString();
        }
        public ICollection<ProductCategory> ProductCategories { get; set; } = new List<ProductCategory>();
    }
}
