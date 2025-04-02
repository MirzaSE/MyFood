using System.ComponentModel.DataAnnotations;

namespace MyFood.Application.Dtos
{
    public class UserRegisterDto
    {
        public string? Username { get; set; }

        public string? Email { get; set; }

        public string? Password { get; set; }

        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }
    }
}