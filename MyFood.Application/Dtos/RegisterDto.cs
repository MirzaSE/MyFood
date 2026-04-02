using System.ComponentModel.DataAnnotations;

namespace MyFood.Application.Dtos;

public class RegisterDto
{
    [Required(ErrorMessage = "User Name is required")]
    public string Username { get; set; }

    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; }
}