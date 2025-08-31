using E_CommerceApp.Migrations;
using E_CommerceApp.Models;
using Microsoft.EntityFrameworkCore;

namespace E_CommerceApp.Repositories
{
    public class AddressRepositorySqlServer : Repository<UserAddresses>
    {
        public AddressRepositorySqlServer(ApplicationDbContext dbContext) : base(dbContext) {
        }
    }
}

/*
 * Here we implement the repository and pass the context we need to the base class so our main methods
 * are used accordingly
 * now we could use the same table with a different context if we wanted to
 */
