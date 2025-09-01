using E_CommerceApp.DTOs;
using E_CommerceApp.Models;
using E_CommerceApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace E_CommerceApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly CityService _cityService;
        private readonly AddressService _addressService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(CityService cityService, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, AddressService addressService)
        {
            _cityService = cityService;
            _userManager = userManager;
            _roleManager = roleManager;
            _addressService = addressService;
        }

        [HttpGet("view-profile")]
        [Authorize]
        public async Task<IActionResult> ViewProfile()
        {

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //User here is ClaimPrinciple User object inside the ControllerBase class
            //When JWT is processed inside the middleware its claims are stored in memory inside the
            //ClaimPrinciple User object

            var user = await _userManager.FindByIdAsync(currentUserId);
            if (user == null) return NotFound("User Not Found");

            string CityName = await _cityService.GetById(user.CityId);

            UserProfileDTO userProfileDTO = new UserProfileDTO();
            userProfileDTO.FirstName = user.FirstName;
            userProfileDTO.LastName = user.LastName;
            userProfileDTO.UserName = user.UserName;
            userProfileDTO.Email = user.Email;
            userProfileDTO.PhoneNumber = user.PhoneNumber;
            userProfileDTO.JoinDate = user.JoinDate;
            userProfileDTO.CityName = CityName;


            //If the id sent is the current logged in user then we don't need to check if user is null
            //because this implicitly means the user exists
            //We don't need to put required constraints on UserProfileDTO as the User is guaranteed to have
            //All his fields so everything will be passed to the DTO

            return Ok(userProfileDTO);

        }

        //Since UserNames are unique this is doable also UX wise username makes more sense in the url than
        //a guid
        [HttpGet("view-user-profile/{userName}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ViewUserProfile(string userName)
        {

            if (!User.IsInRole("Admin")) return Forbid();

            var user = await _userManager.FindByNameAsync(userName);
            if (user == null) return NotFound("User Not Found");

            string CityName = await _cityService.GetById(user.CityId);

            UserProfileDTO userProfileDTO = new UserProfileDTO();
            userProfileDTO.Id = user.Id;
            userProfileDTO.FirstName = user.FirstName;
            userProfileDTO.LastName = user.LastName;
            userProfileDTO.UserName = user.UserName;
            userProfileDTO.Email = user.Email;
            userProfileDTO.PhoneNumber = user.PhoneNumber;
            userProfileDTO.JoinDate = user.JoinDate;
            userProfileDTO.CityName = CityName;

            return Ok(userProfileDTO);

        }

        // This method uses .AsNoTracking(), which improves performance by disabling
        // EF Core's change tracking:
        //  - Lower memory usage per entity (no tracking overhead).
        //  - Faster query execution since EF skips building the tracking graph.
        //  - Better suited for read-only scenarios where entities won't be updated.
        //
        // However, this method still has no pagination.
        // That means if the database contains millions of records:
        //  - The database still has to return all rows.
        //  - Network transfer is still very expensive (all rows must be sent).
        //  - EF still loads all results into memory at once (high memory usage).
        //  - Query time will still be very long.
        //
        // In short: .AsNoTracking() makes reads lighter but does NOT solve the
        // "too much data at once" problem. Pagination is still needed for scalability.

        [Authorize(Roles = "Admin")]
        [HttpGet("get-all-users")]
        public async Task<IActionResult> GetAllUsers()
        {
            //No need to track all those entities in memory we project them into a DTO anyways
            var users = await _userManager.Users.AsNoTracking()
            .Include(u => u.City)
            .Select(u => new UserProfileDTO
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                UserName = u.UserName,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                JoinDate = u.JoinDate,
                CityName = u.City.Name
            })
            .ToListAsync();
            //Select takes each element of a collection (or query) and transforms it into a
            //new shape — like mapping one object into another

            if (users == null) return NotFound("No users found");

            return Ok(users);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("get-users-paginated")]
        public async Task<IActionResult> GetUsersPaginated(int pageNumber = 1)
        {
            var users = await _userManager.Users
            .AsNoTracking()
            .Include(u => u.City)
            .Skip((pageNumber - 1) * 10)
            .Take(10)
            .Select(u => new UserProfileDTO
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                UserName = u.UserName,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                JoinDate = u.JoinDate,
                CityName = u.City.Name
            })
            .ToListAsync();

            if (users == null) return NotFound("No users found");

            //page number is the first set of users, page size is the number of users returned per call
            //so page number 1 would mean first 10 users, page number 2 would mean second 10 users
            //we use .Skip to fetch the page we need and .Take to only take the amount of rows we need

            return Ok(users);
        }

        [HttpDelete("delete-account")]
        [Authorize]
        public async Task<IActionResult> DeleteUser()
        {
            string userName = User.FindFirstValue(ClaimTypes.Name);
            ApplicationUser? user = await _userManager.FindByNameAsync(userName);

            if (user == null) { return NotFound("User doesn't exist"); }

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok($"User {user.UserName} has been deleted");
        }


        [HttpDelete("delete-user/{userName}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(string userName)
        {

            ApplicationUser? user = await _userManager.FindByNameAsync(userName);

            if (user == null) { return NotFound("User doesn't exist"); }

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok($"User {user.UserName} has been deleted");
        }


        [HttpPut("update-profile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile(UserProfileDTO NewProfile)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var validationContext = new ValidationContext(NewProfile,
                serviceProvider: HttpContext.RequestServices,
                items: new Dictionary<object, object?> { { "UserId", currentUserId } });

            var results = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(NewProfile, validationContext, results, true);

            if (!isValid)
            {
                return BadRequest(results);
            }

            ApplicationUser? user = await _userManager.FindByIdAsync(currentUserId);
            if (user == null) return NotFound("User Not Found");

            string CityId = await _cityService.GetIdUsingName(NewProfile.CityName);

            user.FirstName = NewProfile.FirstName;
            user.LastName = NewProfile.LastName;
            user.UserName = NewProfile.UserName;
            user.Email = NewProfile.Email;
            user.PhoneNumber = NewProfile.PhoneNumber;
            user.CityId = CityId;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok("User Profile Updated");
        }

        [HttpPut("update-user-profile/{userName}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateProfile(UserProfileDTO NewProfile, string userName)
        {
            ApplicationUser? user = await _userManager.FindByNameAsync(userName);
            if (user == null) return NotFound("User not found");

            string userId = user.Id;
            var validationContext = new ValidationContext(NewProfile,
                serviceProvider: HttpContext.RequestServices,
                items: new Dictionary<object, object?> { { "UserId", userId } });

            var results = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(NewProfile, validationContext, results, true);

            if (!isValid)
            {
                return BadRequest(results);
            }

            string CityId = await _cityService.GetIdUsingName(NewProfile.CityName);

            user.FirstName = NewProfile.FirstName;
            user.LastName = NewProfile.LastName;
            user.UserName = NewProfile.UserName;
            user.Email = NewProfile.Email;
            user.PhoneNumber = NewProfile.PhoneNumber;
            user.CityId = CityId;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok("User Profile Updated");
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordDTO changePasswordDTO) {
            if (ModelState.IsValid) {

                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                ApplicationUser? user = await _userManager.FindByIdAsync(currentUserId);
                if (user == null) {
                    return NotFound("User doesn't exist");
                }
                var result = await _userManager.ChangePasswordAsync(user, changePasswordDTO.OldPassword, changePasswordDTO.NewPassword);
                if (result.Succeeded)
                {
                    return Ok("Password changed successfully");
                }
                else { 
                    return BadRequest(result.Errors);
                }
            }
            return BadRequest(ModelState);
        }

        [HttpPost("add-address/{address}")]
        [Authorize]
        public async Task<IActionResult> AddAddress(string address) {

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUserName = User.FindFirstValue(ClaimTypes.Name);

            UserAddress userAddress = new UserAddress();
            userAddress.Address = address;
            userAddress.UserId = currentUserId;

            int result = await _addressService.AddAddress(userAddress);
            if (result > 0) {
                return Ok($"Address added for user {currentUserName}");
            }
            return BadRequest($"Address was not added for user {currentUserName}");

        }

        [HttpDelete("remove-address/{addressId}")]
        [Authorize]
        public async Task<IActionResult> RemoveAddress(string addressId) {

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUserName = User.FindFirstValue(ClaimTypes.Name);

            var address = await _addressService.GetById(addressId);
            if (address == null) return NotFound("Address not found");

            int result = await _addressService.RemoveAddress(address);
            if (result > 0)
            {
                return Ok($"Address {address.Address} removed from user {currentUserName}");
            }
            return BadRequest($"Address {address.Address} was not removed from user {currentUserName}");
        }

        [HttpPut("update-address/")]
        [Authorize]
        public async Task<IActionResult> UpdateAddress(AddressDTO addressDTO) {

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUserName = User.FindFirstValue(ClaimTypes.Name);

            var address = await _addressService.GetById(addressDTO.Id);
            if (address == null) return NotFound("Address not found");

            string oldAddress = address.Address;
            address.Address = addressDTO.Address;
            int result = await _addressService.UpdateAddress(address);
            if (result > 0) {
                return Ok($"address for user {currentUserName} updated from {oldAddress} to {address.Address}");
            }
            return BadRequest("Address was not updated");
        }

        [HttpGet("get-addresses")]
        [Authorize]
        public async Task<IActionResult> GetAddresses() {

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUserName = User.FindFirstValue(ClaimTypes.Name);

            List<UserAddress>? addresses = await _addressService.GetAll();
            if (addresses == null) return BadRequest($"No addresses found for user {currentUserName}");

            List<AddressDTO> addressesDTO = new List<AddressDTO>();
            foreach (var address in addresses)
            {
                AddressDTO addressDTO = new AddressDTO();
                addressDTO.Id = address.Id;
                addressDTO.Address = address.Address;
                addressesDTO.Add(addressDTO);
            }

            return Ok(addressesDTO);

        }

        [HttpPut("deassign-role")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeassignRole(UserRoleDTO userRoleDTO) {

            if (ModelState.IsValid)
            {
                ApplicationUser? User = await _userManager.FindByNameAsync(userRoleDTO.UserName);

                if (User == null) { return NotFound("User Doesn't Exist"); }

                bool roleResult = await _roleManager.RoleExistsAsync(userRoleDTO.RoleName);
                if (!roleResult)
                {
                    return NotFound("Role was not found");
                }

                var result = await _userManager.RemoveFromRoleAsync(User, userRoleDTO.RoleName);
                if (result.Succeeded)
                {
                    return Ok($"Role {userRoleDTO.RoleName} removed from user {userRoleDTO.UserName}");
                }
                return BadRequest(result.Errors);
            }
            return BadRequest(ModelState);
        }

        [HttpPost("assign-role")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignRole(UserRoleDTO userRoleDTO)
        {
            if (ModelState.IsValid)
            {

                ApplicationUser? appUser = await _userManager.FindByNameAsync(userRoleDTO.UserName);
                if (appUser == null)
                {
                    return NotFound("Username was not found");
                }
                bool roleResult = await _roleManager.RoleExistsAsync(userRoleDTO.RoleName);
                if (!roleResult)
                {
                    return NotFound("Role was not found");
                }
                var result = await _userManager.AddToRoleAsync(appUser, userRoleDTO.RoleName);
                if (result.Succeeded)
                {
                    return Ok("User assigned to role successfully");
                }
                return BadRequest(result.Errors);
            }
            return BadRequest(ModelState);

        }

    }
}
