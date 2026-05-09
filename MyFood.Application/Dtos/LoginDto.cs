using System.ComponentModel.DataAnnotations;

namespace MyFood.Application.Dtos
{
    public class LoginDto
    {
        [Required(ErrorMessage = "User Name is required")]
        [MinLength(3, ErrorMessage = "Username must be at least 3 characters long.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string Password { get; set; } = string.Empty;
    }
}