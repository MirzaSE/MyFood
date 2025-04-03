using System.ComponentModel.DataAnnotations;

namespace MyFood.Application.Models.Auth
{
    public class LoginDto
{
    [Required]
    [EmailAddress]  // Add if expecting email
    public string Username { get; set; }  // Or rename to Email if needed
    
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }
}
}