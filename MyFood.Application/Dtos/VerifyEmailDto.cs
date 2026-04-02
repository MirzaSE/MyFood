using System.ComponentModel.DataAnnotations;

namespace MyFood.Application.Dtos;

public class VerifyEmailDto
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "A valid email is required.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Verification token is required.")]
    public string Token { get; set; } = string.Empty;
}
