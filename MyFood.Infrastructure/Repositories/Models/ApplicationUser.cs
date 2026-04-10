using Microsoft.AspNetCore.Identity;

namespace MyFood.Infrastructure.Repositories.Models;

public class ApplicationUser : IdentityUser
{
    public string? FullName { get; set; }
    public bool IsEmailVerified { get; set; }
    public string? EmailVerificationToken { get; set; }
    public DateTime? EmailVerificationTokenExpiresAt { get; set; }
}