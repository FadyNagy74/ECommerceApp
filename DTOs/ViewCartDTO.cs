namespace E_CommerceApp.DTOs
{
    public class ViewCartDTO
    {
        public Dictionary<string, int> ProductAndQuantity { get; set; } = new Dictionary<string, int>();
        public decimal TotalPrice { get; set; }

    }
}
