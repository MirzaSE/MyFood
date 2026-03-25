using Microsoft.AspNetCore.Identity;
 
namespace MyFood.Application.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
    }
}