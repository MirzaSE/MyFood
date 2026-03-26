using Microsoft.AspNetCore.Identity;

namespace MyFood.Infrastructure
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
    }
}
