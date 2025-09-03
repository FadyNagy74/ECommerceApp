using System.ComponentModel.DataAnnotations.Schema;

namespace E_CommerceApp.Models
{
    [Table("ProductCategories")]
    public class ProductCategory
    {
        public string ProductId { get; set; }
        public Product Product { get; set; }

        public string CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
