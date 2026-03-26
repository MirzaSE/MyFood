namespace MyFood.Application;

public class RegisterUserDto
{
	public required string Username { get; set; }
	public required string FullName { get; set; }
	public required string Password { get; set; }
}
