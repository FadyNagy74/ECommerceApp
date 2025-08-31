using E_CommerceApp.DTOs;
using E_CommerceApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_CommerceApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public RoleController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            this._roleManager = roleManager;
            this._userManager = userManager;
        }

        [HttpPost("add-role/{RoleName}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddRole(string RoleName)
        {         
                
                IdentityRole identityRole = new IdentityRole();
                identityRole.Name = RoleName;
                IdentityResult result = await _roleManager.CreateAsync(identityRole);
                if (result.Succeeded)
                {
                    return Ok(new { Message = "Role Created Successfully" });
                }
                else
                {
                    return BadRequest(result.Errors);
                }
            
        }

        [HttpDelete("remove-role/{RoleName}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveRole(string RoleName)
        {
      
                var role = await _roleManager.FindByNameAsync(RoleName);
                if (role == null) {
                    return BadRequest($"Role {RoleName} doesn't exist");
                }
                
                var result = await _roleManager.DeleteAsync(role);
                if (result.Succeeded)
                {
                    return Ok(new { Message = "Role deleted successfully" });
                }
                else
                {
                    return BadRequest(result.Errors);
                }
            
        }

        [HttpGet("get-roles")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllRoles() {
            var Roles = await _roleManager.Roles.ToListAsync();
            if (Roles == null) {
                return NotFound("No roles found");
            }
            return Ok(Roles);
        }


    }
}
