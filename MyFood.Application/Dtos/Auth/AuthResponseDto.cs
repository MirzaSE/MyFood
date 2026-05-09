namespace MyFood.Application.Dtos
{
    // This DTO is the response sent back after registration or login
    public class AuthResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public UserDto? User { get; set; }
    }
}
