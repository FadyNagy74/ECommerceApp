using E_CommerceApp.DTOs;
using E_CommerceApp.Models;
using E_CommerceApp.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace E_CommerceApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly CityService _cityService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration config;

        public AuthController(CityService cityService, UserManager<ApplicationUser> userManager, IConfiguration config) { 
            _cityService = cityService;
            _userManager = userManager;
            this.config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDTO user) {

            if (ModelState.IsValid)
            {
                ApplicationUser appUser = new ApplicationUser();
                appUser.FirstName = user.FirstName;
                appUser.LastName = user.LastName;
                appUser.UserName = user.UserName;
                appUser.Email = user.Email;
                appUser.PhoneNumber = user.PhoneNumber;

                string CityId = await _cityService.GetIdUsingName(user.CityName);

                appUser.CityId = CityId;

                IdentityResult registerResult = await _userManager.CreateAsync(appUser, user.Password);

                if (registerResult.Succeeded)
                {
                    await _userManager.AddToRoleAsync(appUser, "User");
                    return Ok("User Registered Successfully");
                }
                else
                {
                    return BadRequest(registerResult.Errors);
                }

            }
            else {
                return BadRequest(ModelState);
            }
            
        }
        
        [HttpPost("Login")]

        public async Task<IActionResult> Login(UserLoginDTO user) {
            if (ModelState.IsValid) {
                ApplicationUser? appUser = await _userManager.FindByNameAsync(user.UserName);
                if (appUser == null)
                {
                    return BadRequest("Username or password is incorrect");
                    //We don't want the user to know which part is incorrect
                    //return NotFound("User not found");
                }
                else
                {
                    bool isValid = await _userManager.CheckPasswordAsync(appUser, user.Password);
                    if (isValid)
                    {

                        List<Claim> UserClaims = new List<Claim>();
                        UserClaims.Add(new Claim(ClaimTypes.NameIdentifier, appUser.Id));
                        UserClaims.Add(new Claim(ClaimTypes.Name, appUser.UserName));
                        UserClaims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));

                        var userRoles = await _userManager.GetRolesAsync(appUser);
                        foreach (var role in userRoles)
                        {
                            UserClaims.Add(new Claim(ClaimTypes.Role, role));
                        }

                        var SignInKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT:SecretKey"]));
                        SigningCredentials signingCredentials = new SigningCredentials(SignInKey, SecurityAlgorithms.HmacSha256);

                        JwtSecurityToken token = new JwtSecurityToken(
                        audience: config["JWT:AudienceIP"],
                        issuer: config["JWT:IssuerIP"],
                        expires: DateTime.Now.AddHours(1),
                        claims: UserClaims,
                        signingCredentials: signingCredentials
                        );

                        return Ok(new
                        {
                            token = new JwtSecurityTokenHandler().WriteToken(token), //Token
                            expiration = DateTime.Now.AddMinutes(60),
                            Message = "Successful Login!"
                        });
                    }
                    else {
                        return BadRequest("Username or password is incorrect");
                    }
                    
                }
            }
            return BadRequest(ModelState);
        }









        /*
        [HttpGet]
        public async Task<IActionResult> SignOut()
        {
            try
            {
                await signInManager.SignOutAsync();
                return Ok(new { Message = "User Signed Out" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Sign Out Failed", Error = ex.Message });
            }
        }
        */
        //There is no signout for JWT as the jwt only exists in the client side and there is no session to be maintained
        //so to signout either wait for teh token to expire or delete the token from the client side

    }
}
