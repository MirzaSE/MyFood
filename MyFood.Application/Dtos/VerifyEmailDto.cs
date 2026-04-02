using System.ComponentModel.DataAnnotations;

namespace MyFood.Application.Dtos;

public class VerifyEmailDto
{
    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string Token { get; set; } = string.Empty;
}
