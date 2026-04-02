using Microsoft.AspNetCore.Identity;

namespace MyFood.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string? VerificationToken { get; set; }
        public DateTime? VerificationTokenExpiry { get; set; }
    }
}