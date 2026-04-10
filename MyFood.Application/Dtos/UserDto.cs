namespace MyFood.Application.Dtos;

public class UserDto
{
    public string? Id { get; set; }
    public string? Username { get; set; }
    public string? Email { get; set; }
    public bool IsEmailVerified { get; set; }
}
