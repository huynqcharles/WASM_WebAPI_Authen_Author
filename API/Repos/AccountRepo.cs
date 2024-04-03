using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Shared.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API.Repos
{
    public class AccountRepo : IAccountRepo
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AccountRepo(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<Response> LoginAsync(LoginDTO loginDTO)
        {
            if (loginDTO is null)
            {
                return new Response(false, null, "Model can not be empty");
            }

            var getUser = await _userManager.FindByNameAsync(loginDTO.Username);
            if (getUser is null)
            {
                return new Response(false, null, "User not found");
            }

            if (!await _userManager.CheckPasswordAsync(getUser, loginDTO.Password))
            {
                return new Response(false, null, "Invalid password");
            }

            // Get list roles of user
            var roles = await _userManager.GetRolesAsync(getUser);

            // Create list of claims
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, getUser.Id.ToString()),
                new Claim(ClaimTypes.Name, getUser.UserName.ToString()),
                new Claim(ClaimTypes.Email, getUser.Email.ToString()),
                new Claim(ClaimTypes.Role, roles.FirstOrDefault())
            };

            // Create JWT Token
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]));
            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                _configuration["JWT:Issuer"],
                _configuration["JWT:Audience"],
                claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: signIn);

            return new Response(true,
                new JwtSecurityTokenHandler().WriteToken(token),
                "Valid user");
        }

        public async Task<Response> RegisterAsync(RegisterDTO registerDTO, string role)
        {
            if (registerDTO is null)
            {
                return new Response(false, null, "Model can not be empty");
            }

            var newUser = new ApplicationUser()
            {
                FirstName = registerDTO.FirstName,
                LastName = registerDTO.LastName,
                UserName = registerDTO.Username,
                Email = registerDTO.Email,
                PasswordHash = registerDTO.Password
            };

            if (await _userManager.FindByEmailAsync(newUser.Email) != null)
            {
                return new Response(false, null, "Email is already exists");
            }

            if (await _userManager.FindByNameAsync(newUser.UserName) != null)
            {
                return new Response(false, null, "Username is already exists");
            }

            var registeredUser = await _userManager.CreateAsync(newUser, registerDTO.Password);
            if (!registeredUser.Succeeded)
            {
                return new Response(false, null, registeredUser.Errors.FirstOrDefault().Description);
            }

            // Assign role to new account
            if (!await _roleManager.RoleExistsAsync(role))
            {
                return new Response(false, null, "This role does not exists");
            }
            else
            {
                await _userManager.AddToRoleAsync(newUser, role);
                return new Response(false, null, "Register successfully");
            }
        }
    }
}
