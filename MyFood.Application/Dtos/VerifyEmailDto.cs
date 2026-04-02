using System.ComponentModel.DataAnnotations;

namespace MyFood.Application.Dtos
{
    public class VerifyEmailDto
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Email format is invalid")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Verification token is required")]
        public string VerificationToken { get; set; } = string.Empty;
    }
}
