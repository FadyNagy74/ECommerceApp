using System.ComponentModel.DataAnnotations.Schema;

namespace E_CommerceApp.Models
{
    [Table("ProductTags")]
    public class ProductTag
    {
        public string ProductId { get; set; }
        public Product Product { get; set; }

        public string TagId { get; set; }
        public Tag Tag { get; set; }
    }
}
