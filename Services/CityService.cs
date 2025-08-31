using E_CommerceApp.Models;
using E_CommerceApp.Repositories;

namespace E_CommerceApp.Services
{
    public class CityService
    {
        private readonly ICityRepository _cityRepository;

        public CityService(ICityRepository cityRepository)
        {
            _cityRepository = cityRepository;
        }
        
        public async Task<string> GetIdUsingName(string name) { 
            string Id = await _cityRepository.GetIdUsingName(name);
            return Id;
        }

        public async Task<string> GetById(string Id) { 
            City? city = await _cityRepository.GetById(Id);
            if (city == null) return "City Not Found";
            return city.Name;
        }
    }
}
