using System.ComponentModel.DataAnnotations;

namespace E_CommerceApp.DTOs
{
    public class ChangePasswordDTO
    {
        [Required]
        public string OldPassword { get; set; }
        [Required]
        public string NewPassword { get; set; }
    }
}
