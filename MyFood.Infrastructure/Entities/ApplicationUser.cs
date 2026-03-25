using Microsoft.AspNetCore.Identity;

namespace MyFood.Infrastructure.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
    }
}
