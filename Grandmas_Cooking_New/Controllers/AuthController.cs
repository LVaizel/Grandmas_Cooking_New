// Gurvansh and John - AuthController.cs created for handling user authentication and registration.

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Grandmas_Cooking_API.Models;
using Grandmas_Cooking_API.ServiceLayer;

namespace Grandmas_Cooking_API.Controllers
{
    /// <summary>
    /// Auth controller 
    /// </summary>

    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly TokenService _tokenService;

        public AuthController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            TokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
        }

       
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            var user = new AppUser
            {
                Name = request.Name,
                UserName = request.UserName,
                Email = request.Email
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            // Optional: assign default role (e.g. "Customer")
            // await _userManager.AddToRoleAsync(user, "Customer");

            return Ok("User registered successfully");
        }

       
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.UserName);

            if (user == null)
            {
                return Unauthorized("Invalid username or password");
            }

            var result = await _signInManager.CheckPasswordSignInAsync(
                user, request.Password, lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                return Unauthorized("Invalid username or password");
            }

            // Get roles from Identity
            var roles = await _userManager.GetRolesAsync(user);

            var token = _tokenService.GenerateJwtToken(user, roles);

            return Ok(token);
        }

    }
}
