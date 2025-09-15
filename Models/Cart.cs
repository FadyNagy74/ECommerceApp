using System.ComponentModel.DataAnnotations.Schema;

namespace E_CommerceApp.Models
{
    [Table("Carts")]
    public class Cart
    {
        public string Id { get; set; }
        public decimal TotalPrice { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public ICollection<CartProduct> CartProducts { get; set; } = new List<CartProduct>();
        public Cart() { 
            Id = Guid.NewGuid().ToString();
        }
    }
}
