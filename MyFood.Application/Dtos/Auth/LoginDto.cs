namespace MyFood.Application.Dtos
{
    // This DTO represents the data client sends when logging in
    public class LoginDto
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
