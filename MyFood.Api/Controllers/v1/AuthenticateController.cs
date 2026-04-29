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
public class AuthenticateController : ControllerBase
{
    private readonly UserManager<ApplicationUser> userManager;
    private readonly RoleManager<IdentityRole> roleManager;
    private readonly IConfiguration _configuration;

    public AuthenticateController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
    {
        this.userManager = userManager;
        this.roleManager = roleManager;
        _configuration = configuration;
    }

    // Helper metoda da ne ponavljamo isti kod dva puta
    private string GenerateToken(ApplicationUser user, IList<string> roles)
    {
        var authClaims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        foreach (var role in roles)
        {
            authClaims.Add(new Claim(ClaimTypes.Role, role));
        }

        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

        var token = new JwtSecurityToken(
            issuer: _configuration["JWT:ValidIssuer"],
            audience: _configuration["JWT:ValidAudience"],
            expires: DateTime.Now.AddHours(3),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto model)
    {
        var user = await userManager.FindByNameAsync(model.Username);

        // FIX: vraća poruku greške umjesto praznog 401
        if (user == null || !await userManager.CheckPasswordAsync(user, model.Password))
            return Unauthorized(new { message = "Invalid username or password." });

        var userRoles = await userManager.GetRolesAsync(user);
        var tokenString = GenerateToken(user, userRoles);

        // FIX: sada vraća i username zajedno s tokenom
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
        var userExists = await userManager.FindByNameAsync(model.Username);

        // FIX: 409 Conflict umjesto 500
        if (userExists != null)
            return Conflict(new { message = "A user with that username already exists." });

        ApplicationUser user = new ApplicationUser()
        {
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = model.Username
        };

        var result = await userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
        {
            // FIX: vraća konkretnu grešku (npr. password prekratak, nema broj, itd.)
            var errors = string.Join(" ", result.Errors.Select(e => e.Description));
            return BadRequest(new { message = errors });
        }

        // FIX: odmah loguje usera — vraća token umjesto samo poruke
        var userRoles = await userManager.GetRolesAsync(user);
        var tokenString = GenerateToken(user, userRoles);

        return Ok(new
        {
            token = tokenString,
            username = user.UserName,
            expiration = DateTime.Now.AddHours(3)
        });
    }
}