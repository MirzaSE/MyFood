namespace MyFood.Application.Models;

public class AuthUser
{
    public string Id { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public bool EmailVerified { get; set; }
    public string EmailVerificationToken { get; set; } = string.Empty;
    public DateTime? EmailVerificationTokenExpiryUtc { get; set; }
}
