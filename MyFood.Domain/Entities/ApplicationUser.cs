using Microsoft.AspNetCore.Identity;

namespace MyFood.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string? EmailVerificationToken { get; set; }
        public DateTime? EmailVerificationTokenExpiry { get; set; }
        public bool IsEmailVerified { get; set; } = false;
    }
}
