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
            if (registerDto == null)
            {
                return Failure("Registration request is required.");
            }

            var username = registerDto.Username?.Trim();
            if (string.IsNullOrWhiteSpace(username))
            {
                return Failure("User Name is required.");
            }

            var email = registerDto.Email?.Trim();
            if (string.IsNullOrWhiteSpace(email))
            {
                return Failure("Email is required.");
            }

            if (string.IsNullOrWhiteSpace(registerDto.Password))
            {
                return Failure("Password is required.");
            }

            if (!_passwordService.IsPasswordStrong(registerDto.Password))
            {
                return Failure(_passwordService.GetPasswordValidationError(registerDto.Password));
            }

            var userExists = await _userRepository.GetByUsernameAsync(username);
            if (userExists != null)
            {
                return Failure("User already exists.");
            }

            var userWithEmail = await _userRepository.GetByEmailAsync(email);
            if (userWithEmail != null)
            {
                return Failure("Email is already in use.");
            }

            var verificationToken = _verificationTokenService.GenerateToken();
            var user = new ApplicationUser
            {
                UserName = username,
                FullName = username,
                Email = email,
                EmailConfirmed = false,
                PasswordHash = _passwordService.HashPassword(registerDto.Password),
                EmailVerificationToken = verificationToken,
                EmailVerificationTokenExpiryUtc = DateTime.UtcNow.AddHours(24),
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var createResult = await _userRepository.CreateAsync(user);
            if (!createResult.Succeeded)
            {
                var errorMessage = string.Join("; ", createResult.Errors);
                return Failure(string.IsNullOrWhiteSpace(errorMessage)
                    ? "User creation failed."
                    : errorMessage);
            }

            await _emailService.SendVerificationEmailAsync(email, verificationToken);

            return new AuthResponseDto
            {
                Success = true,
                Message = "User created successfully. Please verify your email before logging in.",
                User = MapUser(user)
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            if (loginDto == null)
            {
                return Failure("Login request is required.");
            }

            if (string.IsNullOrWhiteSpace(loginDto.Username))
            {
                return Failure("User Name is required.");
            }

            if (string.IsNullOrWhiteSpace(loginDto.Password))
            {
                return Failure("Password is required.");
            }

            var username = loginDto.Username.Trim();
            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null)
            {
                return Failure("Invalid username or password.");
            }

            var isPasswordValid = _passwordService.VerifyPassword(loginDto.Password, user.PasswordHash ?? string.Empty);
            if (!isPasswordValid)
            {
                return Failure("Invalid username or password.");
            }

            if (!user.EmailConfirmed)
            {
                return Failure("Email is not verified. Please verify your email before logging in.");
            }

            var userRoles = await _userRepository.GetRolesAsync(user);
            var token = _tokenService.GenerateToken(user, userRoles);
            if (string.IsNullOrWhiteSpace(token))
            {
                return Failure("Authentication token could not be generated.");
            }

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
            if (verifyEmailDto == null)
            {
                return Failure("Email verification request is required.");
            }

            if (string.IsNullOrWhiteSpace(verifyEmailDto.Email))
            {
                return Failure("Email is required.");
            }

            if (string.IsNullOrWhiteSpace(verifyEmailDto.VerificationToken))
            {
                return Failure("Verification token is required.");
            }

            var email = verifyEmailDto.Email.Trim();
            var token = verifyEmailDto.VerificationToken.Trim();

            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
            {
                return Failure("No user found for this email.");
            }

            if (user.EmailConfirmed)
            {
                return new AuthResponseDto
                {
                    Success = true,
                    Message = "Email is already verified.",
                    User = MapUser(user)
                };
            }

            if (string.IsNullOrWhiteSpace(user.EmailVerificationToken) ||
                !user.EmailVerificationTokenExpiryUtc.HasValue)
            {
                return Failure("Verification token is invalid or expired.");
            }

            if (!_verificationTokenService.ValidateToken(
                    token,
                    user.EmailVerificationToken,
                    user.EmailVerificationTokenExpiryUtc.Value))
            {
                return Failure("Verification token is invalid or expired.");
            }

            user.EmailConfirmed = true;
            user.EmailVerificationToken = null;
            user.EmailVerificationTokenExpiryUtc = null;

            var updateResult = await _userRepository.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                var errorMessage = string.Join("; ", updateResult.Errors);
                return Failure(string.IsNullOrWhiteSpace(errorMessage)
                    ? "Email verification failed."
                    : errorMessage);
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

            var user = await _userRepository.GetByUsernameAsync(username.Trim());
            if (user == null)
            {
                return false;
            }

            if (!user.EmailConfirmed)
            {
                return false;
            }

            return _passwordService.VerifyPassword(password, user.PasswordHash ?? string.Empty);
        }

        private static AuthResponseDto Failure(string message)
        {
            return new AuthResponseDto
            {
                Success = false,
                Message = message
            };
        }

        private static UserDto MapUser(ApplicationUser user)
        {
            return new UserDto
            {
                Id = user.Id,
                Username = user.UserName ?? string.Empty,
                Email = user.Email
            };
        }
    }
}
