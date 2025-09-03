using System.ComponentModel.DataAnnotations.Schema;

namespace E_CommerceApp.Models
{
    [Table("Reviews")]
    public class Review
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastModifiedAt { get; set; }
        public bool IsEdited { get; set; }
        public int RateValue { get; set; }   
        public string ProductId { get; set; }
        public string UserId { get; set; }

        public Product Product { get; set; }
        public ApplicationUser User { get; set; }
        public Review() { 
            Id = Guid.NewGuid().ToString();
            CreatedAt = DateTime.UtcNow;
        }
    }
}
