using System.ComponentModel.DataAnnotations.Schema;

namespace E_CommerceApp.Models
{
    [Table("Cities")]
    public class City
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public ICollection<ApplicationUser> Users { get; set; }

    }
}
