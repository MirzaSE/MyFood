using MyFood.Application.Dtos;
using MyFood.Domain.Entities;

namespace MyFood.Application.Services
{
    public class AuthService : IAuthService
    {
        private const string DefaultUserRole = "User";

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
            if (registerDto is null)
            {
                return Failure("Registration payload is required.");
            }

            if (string.IsNullOrWhiteSpace(registerDto.Username))
            {
                return Failure("Username is required.");
            }

            if (string.IsNullOrWhiteSpace(registerDto.Email))
            {
                return Failure("Email is required.");
            }

            if (!_passwordService.IsPasswordStrong(registerDto.Password))
            {
                return Failure(_passwordService.GetPasswordValidationError(registerDto.Password));
            }

            if (await _userRepository.UserExistsAsync(registerDto.Username, registerDto.Email))
            {
                return Failure("A user with that username or email already exists.");
            }

            var user = new ApplicationUser
            {
                UserName = registerDto.Username.Trim(),
                Email = registerDto.Email.Trim(),
                SecurityStamp = Guid.NewGuid().ToString(),
                PasswordHash = _passwordService.HashPassword(registerDto.Password)
            };

            var result = await _userRepository.CreateAsync(user);
            if (!result.Succeeded)
            {
                return Failure(string.Join(" ", result.Errors));
            }

            await _userRepository.EnsureRoleExistsAsync(DefaultUserRole);
            await _userRepository.AddToRoleAsync(user, DefaultUserRole);

            var roles = await _userRepository.GetRolesAsync(user);
            var token = _tokenService.GenerateToken(user, roles);

            return Success("User created successfully.", user, token);
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            if (loginDto is null)
            {
                return Failure("Login payload is required.");
            }

            if (string.IsNullOrWhiteSpace(loginDto.Username) || string.IsNullOrWhiteSpace(loginDto.Password))
            {
                return Failure("Username and password are required.");
            }

            var user = await _userRepository.GetByUsernameAsync(loginDto.Username.Trim());
            if (user is null || string.IsNullOrWhiteSpace(user.PasswordHash))
            {
                return Failure("Invalid username or password.");
            }

            if (!_passwordService.VerifyPassword(loginDto.Password, user.PasswordHash))
            {
                return Failure("Invalid username or password.");
            }

            var roles = await _userRepository.GetRolesAsync(user);
            var token = _tokenService.GenerateToken(user, roles);

            return Success("Login successful.", user, token);
        }

        public async Task<bool> ValidateCredentialsAsync(string username, string password)
        {
            var user = await _userRepository.GetByUsernameAsync(username);
            return user is not null
                && !string.IsNullOrWhiteSpace(user.PasswordHash)
                && _passwordService.VerifyPassword(password, user.PasswordHash);
        }

        private static AuthResponseDto Failure(string message)
        {
            return new AuthResponseDto
            {
                Success = false,
                Message = message
            };
        }

        private static AuthResponseDto Success(string message, ApplicationUser user, string token)
        {
            return new AuthResponseDto
            {
                Success = true,
                Message = message,
                Token = token,
                User = new UserDto
                {
                    Id = user.Id,
                    Username = user.UserName ?? string.Empty,
                    Email = user.Email
                }
            };
        }
    }
}
