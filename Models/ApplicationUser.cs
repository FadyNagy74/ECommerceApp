using Microsoft.AspNetCore.Identity;

namespace E_CommerceApp.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime JoinDate { get; set; }

        public string CityId { get; set; }
        public City City { get; set; }
        public ICollection<UserAddress> UserAddresses { get; set; }


        public ApplicationUser() {
            JoinDate = DateTime.UtcNow;
        }

    }
}
