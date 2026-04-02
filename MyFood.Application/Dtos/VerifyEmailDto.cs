using System.ComponentModel.DataAnnotations;

namespace MyFood.Application.Dtos
{
    public class VerifyEmailDto
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Email is not valid")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Verification token is required")]
        public string Token { get; set; }
    }
}
