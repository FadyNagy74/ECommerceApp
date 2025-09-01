using System.ComponentModel.DataAnnotations;

namespace E_CommerceApp.DTOs
{
    public class AddressDTO
    {
        [Required]
        public string Id { get; set; }
        [Required]
        public string Address { get; set; }
    }
}
