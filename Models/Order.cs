using System.ComponentModel.DataAnnotations.Schema;

namespace E_CommerceApp.Models
{
    [Table("Orders")]
    public class Order
    {
        public string Id { get; set; }
        public DateTime PlacedAt { get; set; }
        public decimal SubTotal { get; set; }
        public decimal ShippingTotal { get; set; }
        public decimal Tax { get; set; }    //Check in code that it is between 0 and 100
        public decimal Total { get; set; }
        public string ShippingAddress { get; set; }
        public OrderStatus Status { get; set; }
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        public Order() { 
            Id = Guid.NewGuid().ToString();
            PlacedAt = DateTime.UtcNow;
        }
    }

    public enum OrderStatus { 
        Pending,
        Processing,
        Delivered,
        Cancelled,
        Returned
    }
}
