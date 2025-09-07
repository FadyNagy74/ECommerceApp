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

//In this model Tags are predefined entities that must exist in order for a product to be entered
//If each tag belonged to one product then relation should be one to many and not many to many
