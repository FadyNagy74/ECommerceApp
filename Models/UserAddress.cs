using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_CommerceApp.Models
{
    [Table("UserAddresses")]
    public class UserAddress
    {
        public string Id { get; set; }
        [MinLength(5)]
        public string Address { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public UserAddress() {
            Id = Guid.NewGuid().ToString();
        }
    }
}
