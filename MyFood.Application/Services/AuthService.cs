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
                return new AuthResponseDto { Success = false, Message = "Invalid registration request." };
            }

            var username = registerDto.Username?.Trim();
            var email = registerDto.Email?.Trim();
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(registerDto.Password))
            {
                return new AuthResponseDto { Success = false, Message = "Username, email and password are required." };
            }

            if (!_passwordService.IsPasswordStrong(registerDto.Password))
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = _passwordService.GetPasswordValidationError(registerDto.Password)
                };
            }

            var userExists = await _userRepository.GetByUsernameAsync(username);
            if (userExists != null)
            {
                return new AuthResponseDto { Success = false, Message = "User already exists." };
            }

            var emailExists = await _userRepository.GetByEmailAsync(email);
            if (emailExists != null)
            {
                return new AuthResponseDto { Success = false, Message = "Email already exists." };
            }

            var verificationToken = _verificationTokenService.GenerateToken();
            var tokenExpiry = DateTime.UtcNow.AddHours(24);

            var user = new ApplicationUser
            {
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = username,
                Email = email,
                EmailConfirmed = false,
                VerificationToken = verificationToken,
                VerificationTokenExpiry = tokenExpiry
            };

            var result = await _userRepository.CreateAsync(user, registerDto.Password);
            if (!result.Success)
            {
                var errors = string.Join("; ", result.Errors);
                return new AuthResponseDto
                {
                    Success = false,
                    Message = string.IsNullOrWhiteSpace(errors) ? "User creation failed." : errors
                };
            }

            await _emailService.SendVerificationEmailAsync(email, verificationToken);

            return new AuthResponseDto
            {
                Success = true,
                Message = "User created successfully. Please verify your email.",
                User = new UserDto { Id = user.Id, Username = user.UserName ?? string.Empty }
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            if (loginDto == null)
            {
                return new AuthResponseDto { Success = false, Message = "Invalid login request." };
            }

            var username = loginDto.Username?.Trim();
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(loginDto.Password))
            {
                return new AuthResponseDto { Success = false, Message = "Username and password are required." };
            }

            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null)
            {
                return new AuthResponseDto { Success = false, Message = "Invalid username or password." };
            }

            var isValidPassword = await _userRepository.CheckPasswordAsync(user, loginDto.Password);
            if (!isValidPassword)
            {
                return new AuthResponseDto { Success = false, Message = "Invalid username or password." };
            }

            if (!user.EmailConfirmed)
            {
                return new AuthResponseDto { Success = false, Message = "Email is not verified." };
            }

            var roles = await _userRepository.GetRolesAsync(user);
            var token = await _tokenService.GenerateTokenAsync(user, roles);

            if (string.IsNullOrWhiteSpace(token))
            {
                return new AuthResponseDto { Success = false, Message = "Token generation failed." };
            }

            return new AuthResponseDto
            {
                Success = true,
                Message = "Login successful.",
                Token = token,
                User = new UserDto { Id = user.Id, Username = user.UserName ?? string.Empty }
            };
        }

        public async Task<AuthResponseDto> VerifyEmailAsync(VerifyEmailDto verifyEmailDto)
        {
            if (verifyEmailDto == null)
            {
                return new AuthResponseDto { Success = false, Message = "Invalid verification request." };
            }

            var email = verifyEmailDto.Email?.Trim();
            var token = verifyEmailDto.Token?.Trim();

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token))
            {
                return new AuthResponseDto { Success = false, Message = "Email and token are required." };
            }

            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
            {
                return new AuthResponseDto { Success = false, Message = "User not found." };
            }

            if (user.EmailConfirmed)
            {
                return new AuthResponseDto { Success = true, Message = "Email is already verified." };
            }

            if (string.IsNullOrWhiteSpace(user.VerificationToken) || !user.VerificationTokenExpiry.HasValue)
            {
                return new AuthResponseDto { Success = false, Message = "No verification token found for this user." };
            }

            if (!_verificationTokenService.ValidateToken(token, user.VerificationToken, user.VerificationTokenExpiry.Value))
            {
                if (DateTime.UtcNow > user.VerificationTokenExpiry.Value)
                {
                    return new AuthResponseDto { Success = false, Message = "Verification token has expired." };
                }

                return new AuthResponseDto { Success = false, Message = "Verification token is invalid." };
            }

            user.EmailConfirmed = true;
            user.VerificationToken = null;
            user.VerificationTokenExpiry = null;

            var updateResult = await _userRepository.UpdateAsync(user);
            if (!updateResult.Success)
            {
                var errors = string.Join("; ", updateResult.Errors);
                return new AuthResponseDto
                {
                    Success = false,
                    Message = string.IsNullOrWhiteSpace(errors) ? "Email verification failed." : errors
                };
            }

            return new AuthResponseDto { Success = true, Message = "Email verified successfully." };
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

            return await _userRepository.CheckPasswordAsync(user, password);
        }
    }
}
