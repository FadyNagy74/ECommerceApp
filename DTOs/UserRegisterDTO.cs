using E_CommerceApp.CustomValidators;
using System.ComponentModel.DataAnnotations;

namespace E_CommerceApp.DTOs
{
    public class UserRegisterDTO
    {
        [Required]
        [MaxLength(50)]
        [MinLength(3)]
        public string FirstName { get; set; }
        [Required]
        [MaxLength(50)]
        [MinLength(3)]
        public string LastName { get; set; }
        [Required]
        [MaxLength(50)]
        [MinLength(5)]
        [UserNameUnique]
        public string UserName { get; set; }
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [EmailUnique]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        [RegularExpression(@"^(\+20)?(010|011|012|015)[0-9]{8}$", ErrorMessage = "Phone number must be a valid Egyptian number (010, 011, 012, or 015).")]
        [PhoneNumberUnique]
        public string PhoneNumber { get; set; }
        [Required]
        public string CityName { get; set; }
    }
}
