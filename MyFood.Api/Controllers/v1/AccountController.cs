using Microsoft.AspNet.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MyFood.Application.Entities;
using static MyFood.Api.Configuration;

namespace MyFood.Api.Controllers.v1;

public class AccountController : Controller  
 {
 private readonly UserManager<ApplicationUser> _userManager;
 private readonly SignInManager<ApplicationUser> _signInManager;
 private readonly JwtSettings _jwtSettings;

 public AccountController(UserManager<ApplicationUser> userManager,
SignInManager<ApplicationUser> signInManager, IOptions<JwtSettings>
jwtSettings)

 {
 _userManager = userManager;
 _signInManager = signInManager;
         _jwtSettings = jwtSettings.Value;
 }

    // Login method

    public async Task<IActionResult> Login([FromBody] LoginDto model)
 {
 var user = await _userManager.FindByNameAsync(model.Username);
 if (user != null && await _userManager.CheckPasswordAsync(user,
model.Password))

 {
 var userRoles = await _userManager.GetRolesAsync(user);

 var authClaims = new List<Claim>
 {
 new Claim(ClaimTypes.Name, user.UserName),
 new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
 };

 foreach (var userRole in userRoles)
 {
 authClaims.Add(new Claim(ClaimTypes.Role, userRole));
 }

 var authSigningKey = new
SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));


 var token = new JwtSecurityToken(
 issuer: _jwtSettings.ValidIssuer,
 audience: _jwtSettings.ValidAudience,
 expires: DateTime.Now.AddHours(3),
 claims: authClaims,
 signingCredentials: new SigningCredentials(authSigningKey,
SecurityAlgorithms.HmacSha256)
);

 return Ok(new
 {
     token = new JwtSecurityTokenHandler().WriteToken(token),
 expiration = token.ValidTo,
 roles = userRoles
 });
 }
 return Unauthorized();
}
 }
