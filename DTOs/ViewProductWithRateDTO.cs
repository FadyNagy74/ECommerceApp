namespace E_CommerceApp.DTOs
{
    public class ViewProductWithRateDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }

        public double ReviewRate { get; set; }

    }
}
