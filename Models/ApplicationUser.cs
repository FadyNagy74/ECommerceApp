using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace E_CommerceApp.Models
{
    public class ApplicationUser : IdentityUser
    {
        [MinLength(3)]
        public string FirstName { get; set; }
        [MinLength(3)]
        public string LastName { get; set; }
        public DateTime JoinDate { get; set; }

        public string CityId { get; set; }
        public City City { get; set; }
        public ICollection<UserAddress> UserAddresses { get; set; } = new List<UserAddress>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public Cart? Cart { get; set; }


        public ApplicationUser() {
            JoinDate = DateTime.UtcNow;
        }

    }
}
