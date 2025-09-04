using System.ComponentModel.DataAnnotations;

namespace E_CommerceApp.DTOs
{
    public class ProductDTO
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        [Required]
        [MaxLength(2000)]
        public string Description { get; set; }
        [Required]
        public decimal Price { get; set; } //Decimal avoids rounding errors in float or double
        [Required]
        public int Stock { get; set; }
    }
}
