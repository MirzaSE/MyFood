using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MyFood.Application.Dtos;
using MyFood.Domain.Entities;

namespace MyFood.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            var existingUser = await _userManager.FindByNameAsync(registerDto.Username);
            if (existingUser != null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Errors = new[] { "User already exists." }
                };
            }

            var user = new ApplicationUser
            {
                UserName = registerDto.Username,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Errors = result.Errors.Select(e => e.Description)
                };
            }

            return await GenerateAuthResponseAsync(user);
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
                var user = await _userManager.FindByNameAsync(loginDto.Username); //get user
                if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password)) //validate user and check if it exists. Validate password
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Errors = new[] { "Invalid username or password." }
                    }; //return response if auth attempt is invalid.
                }
            return await GenerateAuthResponseAsync(user); // Generate JWT Token and return rsponse
        }

        public async Task<bool> ValidateCredentialsAsync(string username, string password)
        { //validation logic:
            var user = await _userManager.FindByNameAsync(username);
            if (user == null) // see if user is null, if yes, return false
            {
                return false;
            }
            //otherwsie, check password against stored ones
            return await _userManager.CheckPasswordAsync(user, password);
        }

        private async Task<AuthResponseDto> GenerateAuthResponseAsync(ApplicationUser user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var secret = _configuration["JWT:Secret"];
            if (string.IsNullOrWhiteSpace(secret))
            {
                throw new InvalidOperationException("JWT:Secret configuration value is missing.");
            }

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var expires = DateTime.UtcNow.AddHours(3);

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: expires,
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256));

            return new AuthResponseDto
            {
                Success = true,
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                ExpiresAt = expires,
                Errors = null
            };
        }
    }
}
