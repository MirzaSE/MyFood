using System.ComponentModel.DataAnnotations;

namespace MyFood.Application.Dtos
{
    public class UserLoginDto
    {
        public string? Username { get; set; }
        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        public string? Password { get; set; }

    }
}