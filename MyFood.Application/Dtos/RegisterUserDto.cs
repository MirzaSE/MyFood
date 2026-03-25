using System.ComponentModel.DataAnnotations;

namespace MyFood.Application.Dtos
{
    public class RegisterUserDto
    {
        [Required]
        public string? Username { get; set; }

        [Required]
        public string? Password { get; set; }
    }
}
