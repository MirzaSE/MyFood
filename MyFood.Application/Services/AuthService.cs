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
            if (registerDto == null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Invalid registration payload."
                };
            }

            if (string.IsNullOrWhiteSpace(registerDto.Username) ||
                string.IsNullOrWhiteSpace(registerDto.Email) ||
                string.IsNullOrWhiteSpace(registerDto.Password))
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Username, email and password are required."
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

            var existingByUsername = await _userRepository.GetByUsernameAsync(registerDto.Username);
            if (existingByUsername != null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Username already exists."
                };
            }

            var existingByEmail = await _userRepository.GetByEmailAsync(registerDto.Email);
            if (existingByEmail != null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Email already exists."
                };
            }

            var user = new ApplicationUser
            {
                UserName = registerDto.Username.Trim(),
                NormalizedUserName = registerDto.Username.Trim().ToUpperInvariant(),
                Email = registerDto.Email.Trim(),
                NormalizedEmail = registerDto.Email.Trim().ToUpperInvariant(),
                SecurityStamp = Guid.NewGuid().ToString(),
                PasswordHash = _passwordService.HashPassword(registerDto.Password)
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            var token = _tokenService.GenerateToken(user);

            return new AuthResponseDto
            {
                Success = true,
                Message = "User created successfully.",
                Token = token,
                User = new UserDto
                {
                    Username = user.UserName ?? string.Empty,
                    Email = user.Email ?? string.Empty
                }
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            if (loginDto == null ||
                string.IsNullOrWhiteSpace(loginDto.Username) ||
                string.IsNullOrWhiteSpace(loginDto.Password))
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Username and password are required."
                };
            }

            var user = await _userRepository.GetByUsernameAsync(loginDto.Username);
            if (user == null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Invalid username or password."
                };
            }

            var isValid = _passwordService.VerifyPassword(loginDto.Password, user.PasswordHash ?? string.Empty);
            if (!isValid)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Invalid username or password."
                };
            }

            var token = _tokenService.GenerateToken(user);

            return new AuthResponseDto
            {
                Success = true,
                Message = "Login successful.",
                Token = token,
                User = new UserDto
                {
                    Username = user.UserName ?? string.Empty,
                    Email = user.Email ?? string.Empty
                }
            };
        }

        public async Task<bool> ValidateCredentialsAsync(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return false;
            }

            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null)
            {
                return false;
            }

            return _passwordService.VerifyPassword(password, user.PasswordHash ?? string.Empty);
        }
    }
}
