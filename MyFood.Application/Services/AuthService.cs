using MyFood.Application.Dtos;
using MyFood.Application.Repositories;
using MyFood.Domain.Entities;

namespace MyFood.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly IPasswordService _passwordService;
        private readonly IVerificationTokenService _verificationTokenService;
        private readonly IEmailService _emailService;

        public AuthService(
            IUserRepository userRepository,
            ITokenService tokenService,
            IPasswordService passwordService,
            IVerificationTokenService verificationTokenService,
            IEmailService emailService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _passwordService = passwordService;
            _verificationTokenService = verificationTokenService;
            _emailService = emailService;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            if (registerDto is null ||
                string.IsNullOrWhiteSpace(registerDto.Email) ||
                string.IsNullOrWhiteSpace(registerDto.Username) ||
                string.IsNullOrWhiteSpace(registerDto.Password))
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Email, username and password are required."
                };
            }

            var userExists = await _userRepository.FindByUsernameAsync(registerDto.Username);
            var emailExists = await _userRepository.FindByEmailAsync(registerDto.Email);

            if (userExists is not null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "User already exists."
                };
            }

            if (emailExists is not null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Email already exists."
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

            var verificationToken = _verificationTokenService.GenerateToken();
            var verificationTokenExpiry = _verificationTokenService.GetTokenExpiryUtc();

            var user = new ApplicationUser
            {
                Email = registerDto.Email,
                UserName = registerDto.Username,
                SecurityStamp = Guid.NewGuid().ToString(),
                PasswordHash = _passwordService.HashPassword(registerDto.Password),
                IsEmailVerified = false,
                EmailConfirmed = false,
                EmailVerificationToken = verificationToken,
                EmailVerificationTokenExpiresAtUtc = verificationTokenExpiry
            };

            var result = await _userRepository.CreateAsync(user);

            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));

                return new AuthResponseDto
                {
                    Success = false,
                    Message = $"User creation failed. {errors}"
                };
            }

            var verificationLink = $"http://localhost:8080/api/auth/verify-email?email={Uri.EscapeDataString(registerDto.Email)}&token={Uri.EscapeDataString(verificationToken)}";
            await _emailService.SendVerificationEmailAsync(registerDto.Email, verificationLink);

            return new AuthResponseDto
            {
                Success = true,
                Message = "User created successfully. Please verify your email before login."
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            if (loginDto is null ||
                string.IsNullOrWhiteSpace(loginDto.Username) ||
                string.IsNullOrWhiteSpace(loginDto.Password))
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Username and password are required."
                };
            }

            var user = await _userRepository.FindByUsernameAsync(loginDto.Username);
            if (user is null)
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
                    Message = "Email is not verified. Please verify your email before login."
                };
            }

            var isValidPassword = _passwordService.VerifyPassword(loginDto.Password, user.PasswordHash ?? string.Empty);
            if (!isValidPassword)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Invalid username or password."
                };
            }

            var roles = await _userRepository.GetRolesAsync(user);
            var token = _tokenService.GenerateToken(user, roles);

            return new AuthResponseDto
            {
                Success = true,
                Message = "Login successful.",
                Token = token
            };
        }

        public async Task<AuthResponseDto> VerifyEmailAsync(string email, string token)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token))
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Email and token are required."
                };
            }

            var user = await _userRepository.FindByEmailAsync(email);
            if (user is null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Invalid verification request."
                };
            }

            if (user.IsEmailVerified)
            {
                return new AuthResponseDto
                {
                    Success = true,
                    Message = "Email is already verified."
                };
            }

            if (string.IsNullOrWhiteSpace(user.EmailVerificationToken) || user.EmailVerificationTokenExpiresAtUtc is null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Verification token is missing. Please request a new verification email."
                };
            }

            if (DateTime.UtcNow > user.EmailVerificationTokenExpiresAtUtc.Value)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Verification token has expired. Please request a new verification email."
                };
            }

            var isValidToken = _verificationTokenService.ValidateToken(
                token,
                user.EmailVerificationToken,
                user.EmailVerificationTokenExpiresAtUtc.Value);

            if (!isValidToken)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Invalid verification token."
                };
            }

            user.IsEmailVerified = true;
            user.EmailConfirmed = true;
            user.EmailVerifiedAtUtc = DateTime.UtcNow;
            user.EmailVerificationToken = null;
            user.EmailVerificationTokenExpiresAtUtc = null;

            var updateResult = await _userRepository.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                var errors = string.Join("; ", updateResult.Errors.Select(e => e.Description));
                return new AuthResponseDto
                {
                    Success = false,
                    Message = $"Email verification failed. {errors}"
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

            var user = await _userRepository.FindByUsernameAsync(username);
            return user is not null &&
                   user.IsEmailVerified &&
                   _passwordService.VerifyPassword(password, user.PasswordHash ?? string.Empty);
        }
    }
}
