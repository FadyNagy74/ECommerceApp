using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_CommerceApp.Models
{
    [Table("Products")]
    public class Product
    {
        public string Id { get; set; }
        [MinLength(5)]
        public string Name { get; set; }
        [MinLength(10)]
        public string Description { get; set; }
        public decimal Price { get; set; } //Decimal avoids rounding errors in float or double
        public int Stock { get; set; }

        public ICollection<ProductTag> ProductTags { get; set; } = new List<ProductTag>();
        public ICollection<ProductCategory> ProductCategories { get; set; } = new List<ProductCategory>();

        public ICollection<Review> Reviews { get; set; } = new List<Review>();

        public Product() { 
            Id = Guid.NewGuid().ToString();
        }
    }
}
