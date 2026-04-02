using MyFood.Application.Dtos;
using MyFood.Application.Entities;

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
            if (registerDto == null ||
                string.IsNullOrWhiteSpace(registerDto.Username) ||
                string.IsNullOrWhiteSpace(registerDto.Email) ||
                string.IsNullOrWhiteSpace(registerDto.Password))
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Username, email, and password are required."
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
                    Message = "Username already exists."
                };
            }

            var existingEmail = await _userRepository.GetByEmailAsync(registerDto.Email);
            if (existingEmail != null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Email already exists."
                };
            }

            var verificationToken = _verificationTokenService.GenerateToken();
            var verificationTokenExpiry = DateTime.UtcNow.AddHours(24);

            var user = new ApplicationUser
            {
                UserName = registerDto.Username,
                FullName = registerDto.Username,
                Email = registerDto.Email,
                EmailConfirmed = false,
                PasswordHash = _passwordService.HashPassword(registerDto.Password),
                VerificationToken = verificationToken,
                VerificationTokenExpiry = verificationTokenExpiry
            };

            var createResult = await _userRepository.CreateAsync(user);
            if (!createResult.Succeeded)
            {
                var errors = string.Join("; ", createResult.Errors);
                return new AuthResponseDto
                {
                    Success = false,
                    Message = string.IsNullOrWhiteSpace(errors) ? "Registration failed." : errors
                };
            }

            await _emailService.SendVerificationEmailAsync(registerDto.Email, verificationToken);

            return new AuthResponseDto
            {
                Success = true,
                Message = "User registered successfully. Please verify your email before logging in.",
                User = new UserDto
                {
                    Id = user.Id,
                    Username = user.UserName ?? string.Empty,
                    FullName = user.FullName
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

            if (!user.EmailConfirmed)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Email is not verified. Please verify your email before logging in."
                };
            }

            var isPasswordValid = _passwordService.VerifyPassword(loginDto.Password, user.PasswordHash ?? string.Empty);
            if (!isPasswordValid)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Invalid username or password."
                };
            }

            return new AuthResponseDto
            {
                Success = true,
                Message = "Login successful.",
                Token = _tokenService.GenerateToken(user),
                User = new UserDto
                {
                    Id = user.Id,
                    Username = user.UserName ?? string.Empty,
                    FullName = user.FullName
                }
            };
        }

        public async Task<AuthResponseDto> VerifyEmailAsync(VerifyEmailDto verifyEmailDto)
        {
            if (verifyEmailDto == null ||
                string.IsNullOrWhiteSpace(verifyEmailDto.Email) ||
                string.IsNullOrWhiteSpace(verifyEmailDto.Token))
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Email and verification token are required."
                };
            }

            var user = await _userRepository.GetByEmailAsync(verifyEmailDto.Email);
            if (user == null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Invalid verification request."
                };
            }

            if (user.EmailConfirmed)
            {
                return new AuthResponseDto
                {
                    Success = true,
                    Message = "Email is already verified."
                };
            }

            if (user.VerificationTokenExpiry == null || DateTime.UtcNow > user.VerificationTokenExpiry.Value)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Verification token has expired."
                };
            }

            var isTokenValid = _verificationTokenService.ValidateToken(
                verifyEmailDto.Token,
                user.VerificationToken ?? string.Empty,
                user.VerificationTokenExpiry.Value);

            if (!isTokenValid)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Verification token is invalid."
                };
            }

            user.EmailConfirmed = true;
            user.VerificationToken = null;
            user.VerificationTokenExpiry = null;

            var updateResult = await _userRepository.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                var errors = string.Join("; ", updateResult.Errors);
                return new AuthResponseDto
                {
                    Success = false,
                    Message = string.IsNullOrWhiteSpace(errors) ? "Unable to verify email." : errors
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
