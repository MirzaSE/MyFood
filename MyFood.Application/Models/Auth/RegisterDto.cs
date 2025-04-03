using System.ComponentModel.DataAnnotations;

namespace MyFood.Application.Models.Auth
{
    public class RegisterDto
    {
        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; }

        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

        public string FullName { get; set; }
    }
}