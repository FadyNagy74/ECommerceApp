using System.ComponentModel.DataAnnotations;

namespace E_CommerceApp.DTOs
{
    public class UserRoleDTO
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string RoleName { get; set; }
    }
}
