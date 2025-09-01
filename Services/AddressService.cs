using E_CommerceApp.Models;
using E_CommerceApp.Repositories;

namespace E_CommerceApp.Services
{
    public class AddressService
    {
        IAddressRepository _addressRepository;

        public AddressService(IAddressRepository addressRepository)
        {
            _addressRepository = addressRepository;
        }
        public async Task<int> AddAddress(UserAddress address) {
            _addressRepository.Add(address);
            int result = await _addressRepository.SaveChangesAsync();
            return result; 
        }

        public async Task<int> RemoveAddress(UserAddress address) {
            _addressRepository.Remove(address);
            int result = await _addressRepository.SaveChangesAsync();
            return result;
        }

        public async Task<UserAddress?> GetById(string Id) {
            UserAddress? address = await _addressRepository.GetByIdAsync(Id);
            if (address == null) return null;
            return address;
        }

        public async Task<List<UserAddress>?> GetAll() {
            List<UserAddress> addresses = await _addressRepository.GetAllAsync();
            if (addresses == null) return null;
            return addresses;
        }

        public async Task<int> UpdateAddress(UserAddress address) {
            _addressRepository.Update(address);
            int result = await _addressRepository.SaveChangesAsync();
            return result;
        }
    }
}
