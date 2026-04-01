using Microsoft.AspNetCore.Identity;

namespace MyFood.Application.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }

        public bool IsEmailVerified { get; set; }

        public string? EmailVerificationToken { get; set; }

        public DateTime? EmailVerificationTokenExpiry { get; set; }
    }
}