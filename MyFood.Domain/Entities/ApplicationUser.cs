using Microsoft.AspNetCore.Identity;

namespace MyFood.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public string? EmailVerificationToken { get; set; }
        public DateTime? EmailVerificationTokenExpiryUtc { get; set; }
    }
}
