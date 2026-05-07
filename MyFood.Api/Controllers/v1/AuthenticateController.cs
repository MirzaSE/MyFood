using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MyFood.Application.Dtos;
using MyFood.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[Route("api/[controller]")]
[ApiController]
[AllowAnonymous]
public class AuthenticateController : ControllerBase
{
    private readonly UserManager<ApplicationUser> userManager;
    private readonly IConfiguration _configuration;

    public AuthenticateController(UserManager<ApplicationUser> userManager, IConfiguration configuration)
    {
        this.userManager = userManager;
        _configuration = configuration;
    }

    private string GenerateJwtToken(ApplicationUser user, IList<string> roles)
    {
        var authClaims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };
        foreach (var role in roles)
            authClaims.Add(new Claim(ClaimTypes.Role, role));

        var authSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]!));

        return new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(
            issuer: _configuration["JWT:ValidIssuer"],
            audience: _configuration["JWT:ValidAudience"],
            expires: DateTime.Now.AddHours(3),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        ));
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto model)
    {
        if (string.IsNullOrWhiteSpace(model.Username) || string.IsNullOrWhiteSpace(model.Password))
            return BadRequest(new { message = "Username and password are required." });

        var user = await userManager.FindByNameAsync(model.Username);
        if (user == null || !await userManager.CheckPasswordAsync(user, model.Password))
            return Unauthorized(new { message = "Invalid username or password." });

        var roles = await userManager.GetRolesAsync(user);
        var tokenString = GenerateJwtToken(user, roles);

        return Ok(new
        {
            token = tokenString,
            username = user.UserName,
            expiration = DateTime.Now.AddHours(3)
        });
    }

    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserDto model)
    {
        if (string.IsNullOrWhiteSpace(model.Username) || string.IsNullOrWhiteSpace(model.Password))
            return BadRequest(new { message = "Username and password are required." });

        var userExists = await userManager.FindByNameAsync(model.Username);
        if (userExists != null)
            return BadRequest(new { message = "Username already exists. Please choose a different one." });

        ApplicationUser user = new ApplicationUser()
        {
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = model.Username,
            Email = model.Email
        };

        var result = await userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join(" ", result.Errors.Select(e => e.Description));
            return BadRequest(new { message = errors });
        }

        var tokenString = GenerateJwtToken(user, new List<string>());

        return Ok(new
        {
            token = tokenString,
            username = user.UserName,
            message = "User created successfully!"
        });
    }
}