using MyFood.Application.Dtos;
using MyFood.Domain.Entities;

namespace MyFood.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly IPasswordService _passwordService;

        public AuthService(
            IUserRepository userRepository,
            ITokenService tokenService,
            IPasswordService passwordService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _passwordService = passwordService;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            if (string.IsNullOrWhiteSpace(registerDto.Username) || string.IsNullOrWhiteSpace(registerDto.Password))
                return new AuthResponseDto { Success = false, Message = "Username and password are required." };

            var passwordError = _passwordService.GetPasswordValidationError(registerDto.Password);
            if (passwordError != null)
                return new AuthResponseDto { Success = false, Message = passwordError };

            var existingUser = await _userRepository.FindByUsernameAsync(registerDto.Username);
            if (existingUser != null)
                return new AuthResponseDto { Success = false, Message = "User already exists." };

            var user = new ApplicationUser
            {
                UserName = registerDto.Username,
                FullName = registerDto.Username,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var (succeeded, errors) = await _userRepository.CreateUserAsync(user, registerDto.Password);
            if (!succeeded)
                return new AuthResponseDto { Success = false, Message = string.Join(", ", errors) };

            return new AuthResponseDto
            {
                Success = true,
                Message = "User registered successfully.",
                User = new UserDto { Id = user.Id, Username = user.UserName! }
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            if (string.IsNullOrWhiteSpace(loginDto.Username) || string.IsNullOrWhiteSpace(loginDto.Password))
                return new AuthResponseDto { Success = false, Message = "Username and password are required." };

            var user = await _userRepository.FindByUsernameAsync(loginDto.Username);
            if (user == null)
                return new AuthResponseDto { Success = false, Message = "Invalid credentials." };

            var passwordValid = await _userRepository.CheckPasswordAsync(user, loginDto.Password);
            if (!passwordValid)
                return new AuthResponseDto { Success = false, Message = "Invalid credentials." };

            var roles = await _userRepository.GetRolesAsync(user);
            var token = _tokenService.GenerateToken(user, roles);

            return new AuthResponseDto
            {
                Success = true,
                Message = "Login successful.",
                Token = token,
                User = new UserDto { Id = user.Id, Username = user.UserName! }
            };
        }

        public async Task<bool> ValidateCredentialsAsync(string username, string password)
        {
            var user = await _userRepository.FindByUsernameAsync(username);
            if (user == null)
                return false;

            return await _userRepository.CheckPasswordAsync(user, password);
        }
    }
}
