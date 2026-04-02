using Microsoft.AspNetCore.Identity;

namespace MyFood.Application.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public string? VerificationToken { get; set; }
        public DateTime? VerificationTokenExpiry { get; set; }
    }
}
