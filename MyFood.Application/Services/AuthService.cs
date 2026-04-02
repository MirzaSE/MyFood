using System.Threading.Tasks;
using MyFood.Application.Dtos;
using MyFood.Application.Services;

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
            if (registerDto == null || string.IsNullOrWhiteSpace(registerDto.Email) || string.IsNullOrWhiteSpace(registerDto.Password) || string.IsNullOrWhiteSpace(registerDto.FullName))
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Email, password, and full name are required."
                };
            }

            if (!_passwordService.IsPasswordStrong(registerDto.Password))
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = _passwordService.GetPasswordValidationError(registerDto.Password)
                };
            }

            if (await _userRepository.ExistsByEmailAsync(registerDto.Email))
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Email already exists."
                };
            }

            var user = new UserDto
            {
                Email = registerDto.Email,
                FullName = registerDto.FullName
            };
            var passwordHash = _passwordService.HashPassword(registerDto.Password);
            var createdUser = await _userRepository.CreateAsync(user, passwordHash);
            var token = _tokenService.GenerateToken(createdUser);
            return new AuthResponseDto
            {
                Success = true,
                Message = "Registration successful.",
                Token = token,
                User = createdUser
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            if (loginDto == null || string.IsNullOrWhiteSpace(loginDto.Username) || string.IsNullOrWhiteSpace(loginDto.Password))
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Username and password are required."
                };
            }
            var found = await _userRepository.FindForLoginAsync(loginDto.Username!);
            if (found == null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Invalid credentials."
                };
            }

            var (user, passwordHash) = found.Value;
            if (!_passwordService.VerifyPassword(loginDto.Password!, passwordHash))
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Invalid credentials."
                };
            }

            var token = _tokenService.GenerateToken(user);
            return new AuthResponseDto
            {
                Success = true,
                Message = "Login successful.",
                Token = token,
                User = user
            };
        }

        public async Task<bool> ValidateCredentialsAsync(string username, string password)
        {
            var user = await _userRepository.GetByEmailAsync(username);
            if (user == null)
                return false;
            // TODO: Get password hash from user entity (extend UserDto or use domain entity)
            var passwordHash = string.Empty;
            return _passwordService.VerifyPassword(password, passwordHash ?? string.Empty);
        }
    }
}