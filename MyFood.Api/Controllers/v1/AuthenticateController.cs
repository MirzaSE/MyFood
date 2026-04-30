using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MyFood.Application.Dtos;
using MyFood.Domain.Entities;
using System.Collections.Generic;
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

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto model)
    {
        var user = await userManager.FindByNameAsync(model.Username);
        if (user == null || !await userManager.CheckPasswordAsync(user, model.Password))
        {
            return Unauthorized(new { message = "Invalid username or password" });
        }

        var userRoles = await userManager.GetRolesAsync(user);
        return Ok(GenerateAuthenticationResponse(user, userRoles));
    }

    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserDto model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userExists = await userManager.FindByNameAsync(model.Username);
        if (userExists != null)
            return Conflict(new { message = "Username is already taken" });

        ApplicationUser user = new ApplicationUser()
        {
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = model.Username,
            Email = model.Email
        };

        var result = await userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "User creation failed! Please check user details and try again." });

        var userRoles = await userManager.GetRolesAsync(user);
        return Ok(GenerateAuthenticationResponse(user, userRoles));
    }

    private object GenerateAuthenticationResponse(ApplicationUser user, IList<string> userRoles)
    {
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

        return new
        {
            token = new JwtSecurityTokenHandler().WriteToken(token),
            expiration = token.ValidTo,
            username = user.UserName
        };
    }
}