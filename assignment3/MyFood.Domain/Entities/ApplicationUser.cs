using Microsoft.AspNetCore.Identity;

namespace MyFood.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public bool IsEmailVerified { get; set; }
        public string? VerificationToken { get; set; }
        public DateTime? VerificationTokenExpiryTime { get; set; }
    }
}
