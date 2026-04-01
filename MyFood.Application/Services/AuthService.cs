using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MyFood.Application.Dtos;
using MyFood.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MyFood.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IPasswordService _passwordService; // <-- ADDED THIS

        // Update the constructor to accept IPasswordService
        public AuthService(
            UserManager<ApplicationUser> userManager, 
            IConfiguration configuration,
            IPasswordService passwordService) // <-- ADDED THIS
        {
            _userManager = userManager;
            _configuration = configuration;
            _passwordService = passwordService; // <-- ADDED THIS
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterUserDto model)
        {
            // --- ADDED VALIDATION LOGIC ---
            var passwordError = _passwordService.GetPasswordValidationError(model.Password);
            if (!string.IsNullOrEmpty(passwordError))
            {
                return new AuthResponseDto { Success = false, Message = passwordError };
            }
            // ------------------------------

            var userExists = await _userManager.FindByNameAsync(model.Username);
            if (userExists != null)
                return new AuthResponseDto { Success = false, Message = "User exists" };

            ApplicationUser user = new ApplicationUser()
            {
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username,
                FullName = model.Username
            };
            
            var result = await _userManager.CreateAsync(user, model.Password);
            
            if (!result.Succeeded)
                return new AuthResponseDto { Success = false, Message = "User creation failed! Please check user details and try again." };

            return new AuthResponseDto { Success = true, Message = "User created successfully!" };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

                var token = new JwtSecurityToken(
                    issuer: _configuration["JWT:ValidIssuer"],
                    audience: _configuration["JWT:ValidAudience"],
                    expires: DateTime.Now.AddHours(3),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

                return new AuthResponseDto
                {
                    Success = true,
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    Expiration = token.ValidTo
                };
            }

            return new AuthResponseDto { Success = false, Message = "Unauthorized" };
        }
    }
}