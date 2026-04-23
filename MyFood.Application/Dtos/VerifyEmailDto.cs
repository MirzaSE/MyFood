using System.ComponentModel.DataAnnotations;

namespace MyFood.Application.Dtos
{
    public class VerifyEmailDto
    {
        [Required, EmailAddress]
        public required string Email { get; set; }

        [Required]
        public required string Token { get; set; }
    }
}
