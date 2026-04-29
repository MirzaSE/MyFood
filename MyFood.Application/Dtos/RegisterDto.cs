using System.ComponentModel.DataAnnotations;

namespace MyFood.Application.Dtos
{
    public class RegisterUserDto
    {
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(256, MinimumLength = 2, ErrorMessage = "Username must be at least 2 characters.")]
        public string Username { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        [StringLength(256)]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(256, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters.")]
        public string Password { get; set; } = string.Empty;
    }
}
