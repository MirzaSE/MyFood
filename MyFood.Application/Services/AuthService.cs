using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyFood.Application.Dtos;
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
                return FailureResponse("Registration payload is required.");
            }

            if (string.IsNullOrWhiteSpace(registerDto.Username))
            {
                return FailureResponse("Username is required.");
            }

            if (string.IsNullOrWhiteSpace(registerDto.Email))
            {
                return FailureResponse("Email is required.");
            }

            var passwordResult = _passwordService.ValidateStrength(registerDto.Password);
            if (!passwordResult.IsStrong)
            {
                return FailureResponse(passwordResult.Errors);
            }

            var existingUser = await _userRepository.FindByUsernameAsync(registerDto.Username);
            if (existingUser != null)
            {
                return FailureResponse("User already exists.");
            }

            var existingEmailUser = await _userRepository.FindByEmailAsync(registerDto.Email);
            if (existingEmailUser != null)
            {
                return FailureResponse("Email is already registered.");
            }

            var user = new ApplicationUser
            {
                UserName = registerDto.Username,
                Email = registerDto.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                IsEmailVerified = false,
                EmailConfirmed = false
            };

            var result = await _userRepository.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                return FailureResponse(result.Errors.Select(e => e.Description));
            }

            var verificationToken = _verificationTokenService.GenerateToken();
            user.EmailVerificationToken = verificationToken;
            user.EmailVerificationTokenExpiresAt = DateTime.UtcNow.AddHours(24);
            await _userRepository.UpdateAsync(user);
            await _emailService.SendVerificationEmailAsync(registerDto.Email, verificationToken);

            return SuccessResponse("Registration successful. Please verify your email to continue.");
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            if (loginDto == null)
            {
                return FailureResponse("Login payload is required.");
            }

            if (string.IsNullOrWhiteSpace(loginDto.Username) || string.IsNullOrWhiteSpace(loginDto.Password))
            {
                return FailureResponse("Username and password are required.");
            }

            var user = await _userRepository.FindByUsernameAsync(loginDto.Username);
            if (user == null || !await _userRepository.CheckPasswordAsync(user, loginDto.Password))
            {
                return FailureResponse("Invalid username or password.");
            }

            if (!user.IsEmailVerified)
            {
                return FailureResponse("Please verify your email address before logging in.");
            }

            return await GenerateAuthResponseAsync(user);
        }

        public async Task<AuthResponseDto> VerifyEmailAsync(VerifyEmailDto verifyEmailDto)
        {
            if (verifyEmailDto == null)
            {
                return FailureResponse("Verification payload is required.");
            }

            var user = await _userRepository.FindByEmailAsync(verifyEmailDto.Email);
            if (user == null)
            {
                return FailureResponse("User with the provided email was not found.");
            }

            if (user.IsEmailVerified)
            {
                return SuccessResponse("Email is already verified.");
            }

            if (string.IsNullOrWhiteSpace(user.EmailVerificationToken) ||
                !user.EmailVerificationTokenExpiresAt.HasValue)
            {
                return FailureResponse("No verification token is associated with this account.");
            }

            var expiresAt = user.EmailVerificationTokenExpiresAt.Value;
            if (!_verificationTokenService.ValidateToken(
                    verifyEmailDto.Token,
                    user.EmailVerificationToken,
                    expiresAt))
            {
                return FailureResponse("Invalid or expired verification token.");
            }

            user.IsEmailVerified = true;
            user.EmailConfirmed = true;
            user.EmailVerificationToken = null;
            user.EmailVerificationTokenExpiresAt = null;
            await _userRepository.UpdateAsync(user);

            return SuccessResponse("Email verified successfully. You can now log in.");
        }

        public async Task<bool> ValidateCredentialsAsync(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return false;
            }

            var user = await _userRepository.FindByUsernameAsync(username);
            if (user == null)
            {
                return false;
            }

            return await _userRepository.CheckPasswordAsync(user, password);
        }

        private async Task<AuthResponseDto> GenerateAuthResponseAsync(ApplicationUser user)
        {
            var userRoles = await _userRepository.GetRolesAsync(user);
            var tokenResult = await _tokenService.GenerateTokenAsync(user, userRoles);

            return new AuthResponseDto
            {
                Success = true,
                Token = tokenResult.Token,
                ExpiresAt = tokenResult.ExpiresAt
            };
        }

        private static AuthResponseDto FailureResponse(params string[] errors)
            => FailureResponse(errors?.AsEnumerable() ?? Array.Empty<string>());

        private static AuthResponseDto FailureResponse(IEnumerable<string> errors)
        {
            var sanitizedErrors = errors?
                .Where(e => !string.IsNullOrWhiteSpace(e))
                .ToArray();

            return new AuthResponseDto
            {
                Success = false,
                Errors = sanitizedErrors?.Length > 0 ? sanitizedErrors : new[] { "Authentication request failed." }
            };
        }

        private static AuthResponseDto SuccessResponse(string message)
        {
            return new AuthResponseDto
            {
                Success = true,
                Message = message
            };
        }
    }
}
