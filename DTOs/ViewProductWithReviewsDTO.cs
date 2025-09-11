namespace E_CommerceApp.DTOs
{
    public class ViewProductWithReviewsDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }

        public List<ShowReviewDTO>? ShowReviews { get; set; }

    }

}
