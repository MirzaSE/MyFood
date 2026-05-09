namespace MyFood.Application.Dtos
{
    // This DTO represents the data client sends when registering
    public class RegisterDto
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;  // Add email - good for identity
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;  // Added for validation
        public string FullName { get; set; } = string.Empty;
    }
}
