using E_CommerceApp.Models;
using Microsoft.EntityFrameworkCore;

namespace E_CommerceApp.Repositories
{
    public class CityRepository : Repository<City>, ICityRepository
    {       
        public CityRepository(ApplicationDbContext context) : base(context) { 
        }

        public async Task<string> GetIdUsingName(string name) {
            City? city = await _dbSet.FirstOrDefaultAsync(c => c.Name == name);
            if (city != null) return city.Id.ToString();
            else throw new KeyNotFoundException($"City with name '{name}' was not found.");
        }
        //City names will be shown to user using a dropdown menu so user won't have an option to mess up

       
    }
}
