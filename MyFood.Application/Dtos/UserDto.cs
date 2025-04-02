namespace MyFood.Application.Dtos
{
    public class RegisterDto
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}

namespace MyFood.Application.Dtos
{
    public class LoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
