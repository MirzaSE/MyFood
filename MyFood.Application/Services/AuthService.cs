using MyFood.Application.Dtos;
using MyFood.Application.Entities;
using MyFood.Application.Repositories;

namespace MyFood.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly IPasswordService _passwordService;
        private readonly IEmailService _emailService;
        private readonly IVerificationTokenService _verificationTokenService;

        public AuthService(
            IUserRepository userRepository,
            ITokenService tokenService,
            IPasswordService passwordService,
            IEmailService emailService,
            IVerificationTokenService verificationTokenService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _passwordService = passwordService;
            _emailService = emailService;
            _verificationTokenService = verificationTokenService;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            if (string.IsNullOrWhiteSpace(registerDto.Username)
                || string.IsNullOrWhiteSpace(registerDto.Email)
                || string.IsNullOrWhiteSpace(registerDto.Password))
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

            var existingUser = await _userRepository.GetByUsernameAsync(registerDto.Username);
            if (existingUser != null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "User already exists."
                };
            }

            var existingEmail = await _userRepository.GetByEmailAsync(registerDto.Email);
            if (existingEmail != null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Email is already in use."
                };
            }

            var verificationToken = _verificationTokenService.GenerateToken();

            var user = new ApplicationUser
            {
                UserName = registerDto.Username,
                Email = registerDto.Email,
                FullName = string.IsNullOrWhiteSpace(registerDto.FullName) ? registerDto.Username : registerDto.FullName,
                PasswordHash = _passwordService.HashPassword(registerDto.Password),
                IsEmailVerified = false,
                EmailVerificationToken = verificationToken,
                EmailVerificationTokenExpiry = DateTime.UtcNow.AddHours(24)
            };

            var result = await _userRepository.CreateAsync(user);
            if (!result.Succeeded)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = string.Join("; ", result.Errors.Select(x => x.Description))
                };
            }

            await _emailService.SendVerificationEmailAsync(user.Email!, verificationToken);

            return new AuthResponseDto
            {
                Success = true,
                Message = "User registered successfully. Please verify your email.",
                User = MapUser(user)
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            if (string.IsNullOrWhiteSpace(loginDto.Username) || string.IsNullOrWhiteSpace(loginDto.Password))
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

            if (!user.IsEmailVerified)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Email is not verified. Please verify your email before logging in."
                };
            }

            var credentialsValid = _passwordService.VerifyPassword(loginDto.Password, user.PasswordHash ?? string.Empty);
            if (!credentialsValid)
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
                User = MapUser(user)
            };
        }

        public async Task<AuthResponseDto> VerifyEmailAsync(VerifyEmailDto verifyEmailDto)
        {
            if (string.IsNullOrWhiteSpace(verifyEmailDto.Email) || string.IsNullOrWhiteSpace(verifyEmailDto.Token))
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Email and token are required."
                };
            }

            var user = await _userRepository.GetByEmailAsync(verifyEmailDto.Email);
            if (user == null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "User not found."
                };
            }

            if (user.IsEmailVerified)
            {
                return new AuthResponseDto
                {
                    Success = true,
                    Message = "Email is already verified.",
                    User = MapUser(user)
                };
            }

            if (string.IsNullOrWhiteSpace(user.EmailVerificationToken) || user.EmailVerificationTokenExpiry == null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Verification token is missing. Please register again."
                };
            }

            var tokenValid = _verificationTokenService.ValidateToken(
                verifyEmailDto.Token,
                user.EmailVerificationToken,
                user.EmailVerificationTokenExpiry.Value);

            if (!tokenValid)
            {
                var expired = user.EmailVerificationTokenExpiry.Value < DateTime.UtcNow;
                return new AuthResponseDto
                {
                    Success = false,
                    Message = expired
                        ? "Verification token has expired. Please request a new verification email."
                        : "Invalid verification token."
                };
            }

            user.IsEmailVerified = true;
            user.EmailVerificationToken = null;
            user.EmailVerificationTokenExpiry = null;

            var updateResult = await _userRepository.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Failed to verify email."
                };
            }

            return new AuthResponseDto
            {
                Success = true,
                Message = "Email verified successfully.",
                User = MapUser(user)
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

        private static UserDto MapUser(ApplicationUser user)
        {
            return new UserDto
            {
                Id = user.Id,
                Username = user.UserName,
                FullName = user.FullName
            };
        }
    }
}