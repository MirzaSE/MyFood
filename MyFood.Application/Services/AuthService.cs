using MyFood.Application.Dtos;
using MyFood.Domain.Entities;
using System;
using System.Threading.Tasks;

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
            // Validate passwords match
            if (registerDto.Password != registerDto.ConfirmPassword)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Passwords do not match"
                };
            }

            // Validate password strength
            var passwordError = _passwordService.GetPasswordValidationError(registerDto.Password);
            if (passwordError != null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = passwordError
                };
            }

            // Check if user already exists
            var existingUser = await _userRepository.FindByUsernameAsync(registerDto.Username);
            if (existingUser != null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Username already exists"
                };
            }

            // Check if email already exists
            var existingEmail = await _userRepository.FindByEmailAsync(registerDto.Email);
            if (existingEmail != null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Email already exists"
                };
            }

            // Create the user
            var user = new ApplicationUser
            {
                UserName = registerDto.Username,
                Email = registerDto.Email,
                FullName = registerDto.FullName,
                IsEmailVerified = false
            };

            var createResult = await _userRepository.CreateAsync(user, registerDto.Password);
            
            if (!createResult.Succeeded)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = string.Join(", ", createResult.Errors)
                };
            }

            // Generate and save verification token
            var verificationToken = _verificationTokenService.GenerateToken();
            var expiryTime = DateTime.UtcNow.AddHours(24);
            
            await _userRepository.SetEmailVerificationTokenAsync(user.Id, verificationToken, expiryTime);
            
            // Send verification email
            await _emailService.SendVerificationEmailAsync(user.Email, verificationToken, user.Id);

            return new AuthResponseDto
            {
                Success = true,
                Message = "Registration successful. Please check your email to verify your account.",
                Token = null, // No token until email verified
                User = new UserDto
                {
                    Id = user.Id,
                    Username = user.UserName,
                    Email = user.Email ?? string.Empty,
                    FullName = user.FullName
                }
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            if (string.IsNullOrWhiteSpace(loginDto.Username))
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Username is required"
                };
            }

            if (string.IsNullOrWhiteSpace(loginDto.Password))
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Password is required"
                };
            }

            var user = await _userRepository.FindByUsernameAsync(loginDto.Username);

            if (user == null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Invalid username or password"
                };
            }

            // Check if email is verified
            if (!user.IsEmailVerified)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Please verify your email before logging in"
                };
            }

            var isValidPassword = await _userRepository.CheckPasswordAsync(user, loginDto.Password);
            
            if (!isValidPassword)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Invalid username or password"
                };
            }

            var token = await _tokenService.GenerateTokenAsync(user);

            return new AuthResponseDto
            {
                Success = true,
                Message = "Login successful",
                Token = token,
                User = new UserDto
                {
                    Id = user.Id,
                    Username = user.UserName,
                    Email = user.Email ?? string.Empty,
                    FullName = user.FullName
                }
            };
        }

        // Add new method for email verification
        public async Task<AuthResponseDto> VerifyEmailAsync(string userId, string token)
        {
            var user = await _userRepository.FindByIdAsync(userId);
            if (user == null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Invalid user"
                };
            }

            var verified = await _userRepository.VerifyEmailAsync(userId, token);
            
            if (!verified)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Invalid or expired verification token"
                };
            }

            return new AuthResponseDto
            {
                Success = true,
                Message = "Email verified successfully. You can now log in."
            };
        }

        public async Task<bool> ValidateCredentialsAsync(string username, string password)
        {
            var user = await _userRepository.FindByUsernameAsync(username);
            
            if (user == null || !user.IsEmailVerified)
            {
                return false;
            }
            
            return await _userRepository.CheckPasswordAsync(user, password);
        }
    }
}
