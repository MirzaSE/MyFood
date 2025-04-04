using Azure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MyFood.Application.Dtos;
using MyFood.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MyFood.Api.Controllers.v1
{

   
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly JwtSettings _jwtSettings;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<JwtSettings> jwtSettings,
            ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _jwtSettings = jwtSettings.Value;
            _logger = logger;
        }

       
        
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto userRegister)
        {
           

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (userRegister.Password != userRegister.ConfirmPassword)
            {
                _logger.LogWarning("Password mismatch for user: {Email}", userRegister.Email);
                return BadRequest(new { Message = "Passwords do not match" });
            }

            var userExists = await _userManager.FindByEmailAsync(userRegister.Email);
            if (userExists != null)
            {
                _logger.LogWarning("User registration failed. Email already exists: {Email}", userRegister.Email);
                return Conflict(new { Message = "User already exists" });
            }

            var user = new ApplicationUser
            {
                UserName = userRegister.Username,
                Email = userRegister.Email,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var result = await _userManager.CreateAsync(user, userRegister.Password);
            if (!result.Succeeded)
            {
                _logger.LogError("User registration failed for {Email}. Errors: {Errors}", userRegister.Email, string.Join(", ", result.Errors.Select(e => e.Description)));
                return BadRequest(result.Errors);
            };

            if (!await _roleManager.RoleExistsAsync("User"))
            {
                await _roleManager.CreateAsync(new IdentityRole("User"));
            }
            await _userManager.AddToRoleAsync(user, "User");

            _logger.LogInformation("User registered successfully: {Email}", userRegister.Email);
            return Ok(new { Message = "User created successfully!" });
        }

        
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto userLogin)
        {
          

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(userLogin.Email);
            if (user == null)
            {
                _logger.LogWarning("Login failed. Email not found: {Email}", userLogin.Email);
                return Unauthorized(new { message = "Invalid email" });
            }

            if (!await _userManager.CheckPasswordAsync(user, userLogin.Password))
            {
                _logger.LogWarning("Login failed. Incorrect password for: {Email}", userLogin.Email);
                return Unauthorized(new { message = "Invalid password" });
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var token = GenerateJwtToken(user, userRoles);

            _logger.LogInformation("User logged in successfully:{UserName} {Email}, Roles: {Roles}",user.UserName, user.Email, string.Join(", ", userRoles));
          


            return Ok(new
            {
                
                token = token,
                expiration = DateTime.UtcNow.AddHours(3),
                roles = userRoles,
                username = user.UserName 
            });
        }


        private string GenerateJwtToken(ApplicationUser user, IList<string> roles)
        {
            _logger.LogInformation("Generating JWT token for user: {Email}", user.Email);

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach (var role in roles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.ValidIssuer,
                audience: _jwtSettings.ValidAudience,
                expires: DateTime.UtcNow.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            _logger.LogInformation("JWT token generated successfully for user: {Email}", user.Email);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
