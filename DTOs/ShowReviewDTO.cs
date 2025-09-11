namespace E_CommerceApp.DTOs
{
    public class ShowReviewDTO
    {
        public string ReviewOwner { get; set; }
        public string Review { get; set; }
        public int RateValue { get; set; }
        public bool IsEdited { get; set; }
    }
}
