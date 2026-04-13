using MyFood.Application.Dtos;
using MyFood.Domain.Entities;

namespace MyFood.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly IPasswordService _passwordService;

        public AuthService(IUserRepository userRepository, ITokenService tokenService, IPasswordService passwordService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _passwordService = passwordService;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterUserDto registerDto)
        {
            var validationError = _passwordService.GetPasswordValidationError(registerDto.Password);
            if (validationError != null)
                return new AuthResponseDto { Success = false, Message = validationError };

            if (await _userRepository.UserExistsAsync(registerDto.Username))
                return new AuthResponseDto { Success = false, Message = "User already exists." };

            var user = new ApplicationUser
            {
                UserName = registerDto.Username,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var (success, error) = await _userRepository.CreateUserAsync(user, registerDto.Password);
            if (!success)
                return new AuthResponseDto { Success = false, Message = error ?? "User creation failed." };

            return new AuthResponseDto
            {
                Success = true,
                Message = "User created successfully.",
                User = new UserDto { Id = user.Id, Username = user.UserName! }
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _userRepository.GetUserByUsernameAsync(loginDto.Username);
            if (user == null)
                return new AuthResponseDto { Success = false, Message = "Invalid credentials." };

            if (!await _userRepository.CheckPasswordAsync(user, loginDto.Password))
                return new AuthResponseDto { Success = false, Message = "Invalid credentials." };

            var roles = await _userRepository.GetUserRolesAsync(user);
            var token = _tokenService.GenerateToken(user, roles);

            return new AuthResponseDto
            {
                Success = true,
                Message = "Login successful.",
                Token = token,
                User = new UserDto { Id = user.Id, Username = user.UserName!, Email = user.Email }
            };
        }

        public async Task<bool> ValidateCredentialsAsync(string username, string password)
        {
            var user = await _userRepository.GetUserByUsernameAsync(username);
            if (user == null) return false;
            return await _userRepository.CheckPasswordAsync(user, password);
        }
    }
}
