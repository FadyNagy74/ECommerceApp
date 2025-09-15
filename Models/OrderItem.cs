using System.ComponentModel.DataAnnotations.Schema;

namespace E_CommerceApp.Models
{
    [Table("OrderItems")]
    public class OrderItem
    {
        public string Id { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public string OrderId { get; set; }
        public Order Order { get; set; }
        public string? ProductId { get; set; }
        public Product? Product { get; set; }
        public OrderItem() { 
            Id = Guid.NewGuid().ToString();
        }
    }
}
