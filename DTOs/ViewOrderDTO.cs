using E_CommerceApp.Models;

namespace E_CommerceApp.DTOs
{
    public class ViewOrderDTO
    {
        public DateTime PlacedAt { get; set; }
        public decimal SubTotal { get; set; }
        public decimal ShippingTotal { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }
        public string ShippingAddress { get; set; }
        public string Status { get; set; }
        public List<OrderItemDTO> OrderItemDTOs { get; set; } = new List<OrderItemDTO>();
    }
}
