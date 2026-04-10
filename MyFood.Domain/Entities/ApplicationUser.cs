using System;
using Microsoft.AspNetCore.Identity;

namespace MyFood.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public bool IsEmailVerified { get; set; }
        public string? EmailVerificationToken { get; set; }
        public DateTime? EmailVerificationTokenExpiresAt { get; set; }
        public string? FullName { get; set; }
    }
}
