using Microsoft.AspNetCore.Identity;

namespace MyFood.Infrastructure.Repositories
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
    }
}
