using MyFood.Application.Dtos;
using MyFood.Application.Repositories;
using MyFood.Domain.Entities;

namespace MyFood.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordService _passwordService;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;
        private readonly IVerificationTokenService _verificationTokenService;

        public AuthService(
            IUserRepository userRepository,
            IPasswordService passwordService,
            ITokenService tokenService,
            IEmailService emailService,
            IVerificationTokenService verificationTokenService)
        {
            _userRepository = userRepository;
            _passwordService = passwordService;
            _tokenService = tokenService;
            _emailService = emailService;
            _verificationTokenService = verificationTokenService;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            if (registerDto == null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Request body is required."
                };
            }

            if (string.IsNullOrWhiteSpace(registerDto.Username)
                || string.IsNullOrWhiteSpace(registerDto.Password)
                || string.IsNullOrWhiteSpace(registerDto.Email))
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Username, password, and email are required."
                };
            }

            var passwordValidationError = _passwordService.GetPasswordValidationError(registerDto.Password);
            if (!string.IsNullOrEmpty(passwordValidationError))
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = passwordValidationError
                };
            }

            var existingUser = await _userRepository.GetByUsernameAsync(registerDto.Username);
            if (existingUser != null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "User already exists."
                };
            }

            var newUser = new ApplicationUser
            {
                Email = registerDto.Email,
                EmailConfirmed = false,
                EmailVerificationToken = _verificationTokenService.GenerateToken(),
                EmailVerificationTokenExpiry = DateTime.UtcNow.AddHours(24),
                FullName = registerDto.Username,
                PasswordHash = _passwordService.HashPassword(registerDto.Password),
                UserName = registerDto.Username,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var result = await _userRepository.CreateAsync(newUser);
            if (!result.Succeeded)
            {
                var errorMessage = string.Join("; ", result.Errors);
                return new AuthResponseDto
                {
                    Success = false,
                    Message = string.IsNullOrWhiteSpace(errorMessage) ? "User registration failed." : errorMessage
                };
            }

            if (!string.IsNullOrWhiteSpace(newUser.EmailVerificationToken))
            {
                await _userRepository.SaveEmailVerificationTokenAsync(newUser, newUser.EmailVerificationToken);
            }

            await _emailService.SendVerificationEmailAsync(registerDto.Email, newUser.EmailVerificationToken!);

            return new AuthResponseDto
            {
                Success = true,
                Message = "Registration successful. Please check your email to verify your account.",
                User = new UserDto
                {
                    Id = newUser.Id,
                    Username = newUser.UserName ?? string.Empty,
                    Email = newUser.Email ?? string.Empty
                }
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

            var user = await _userRepository.GetByUsernameAsync(loginDto.Username);
            if (user == null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Invalid credentials."
                };
            }

            if (!user.EmailConfirmed)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Email address is not verified. Please verify your email before logging in."
                };
            }

            var validPassword = _passwordService.VerifyPassword(loginDto.Password, user.PasswordHash ?? string.Empty);
            if (!validPassword)
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
                User = new UserDto
                {
                    Id = user.Id,
                    Username = user.UserName ?? string.Empty,
                    Email = user.Email ?? string.Empty
                }
            };
        }

        public async Task<AuthResponseDto> VerifyEmailAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Verification token is required."
                };
            }

            var user = await _userRepository.GetByEmailVerificationTokenAsync(token);
            if (user == null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Invalid verification token."
                };
            }

            if (!_verificationTokenService.IsTokenValid(user.EmailVerificationTokenExpiry))
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Verification token has expired."
                };
            }

            user.EmailConfirmed = true;
            user.EmailVerificationToken = null;
            user.EmailVerificationTokenExpiry = null;

            await _userRepository.RemoveEmailVerificationTokenAsync(user);

            var result = await _userRepository.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errorMessage = string.Join("; ", result.Errors);
                return new AuthResponseDto
                {
                    Success = false,
                    Message = string.IsNullOrWhiteSpace(errorMessage) ? "Email verification failed." : errorMessage
                };
            }

            return new AuthResponseDto
            {
                Success = true,
                Message = "Email verified successfully."
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
