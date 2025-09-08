using E_CommerceApp.Models;
using System.ComponentModel.DataAnnotations;

namespace E_CommerceApp.DTOs
{
    public class ReviewDTO
    {
        [MinLength(5)]
        [MaxLength(2000)]
        [Required]
        public string Description { get; set; }
        [Required]
        [Range(1, 5, ErrorMessage = "Rate value must be a whole number between 1 and 5.")]
        public int RateValue { get; set; }
      
    }
}
