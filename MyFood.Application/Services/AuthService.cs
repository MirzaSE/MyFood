using MyFood.Application.Dtos;
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
            if (registerDto == null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Invalid registration data"
                };
            }

            var userExists = await _userRepository.UsernameExistsAsync(registerDto.Username);
            if (userExists)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "User already exists"
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

            var user = new ApplicationUser
            {
                UserName = registerDto.Username,
                Email = registerDto.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                EmailVerificationToken = verificationToken,
                EmailVerificationTokenExpiry = DateTime.UtcNow.AddHours(24),
                IsEmailVerified = false
            };

            try
            {
                var createdUser = await _userRepository.CreateAsync(user, registerDto.Password);
                await _emailService.SendVerificationEmailAsync(createdUser.Email ?? string.Empty, verificationToken);

                return new AuthResponseDto
                {
                    Success = true,
                    Message = "User created successfully. Please check your email to verify your account.",
                    User = new UserDto
                    {
                        Id = createdUser.Id,
                        Username = createdUser.UserName ?? string.Empty,
                        Email = createdUser.Email ?? string.Empty
                    }
                };
            }
            catch (Exception ex)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = $"User creation failed: {ex.Message}"
                };
            }
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            if (loginDto == null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Invalid login data"
                };
            }

            var user = await _userRepository.GetByUsernameAsync(loginDto.Username);
            if (user == null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Invalid credentials"
                };
            }

            if (!user.IsEmailVerified)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Please verify your email before logging in."
                };
            }

            var isValid = await _userRepository.CheckPasswordAsync(user, loginDto.Password);
            if (!isValid)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Invalid credentials"
                };
            }

            var roles = await _userRepository.GetUserRolesAsync(user);
            var token = _tokenService.GenerateJwtToken(user, roles);

            return new AuthResponseDto
            {
                Success = true,
                Message = "Login successful",
                Token = token,
                User = new UserDto
                {
                    Id = user.Id,
                    Username = user.UserName ?? string.Empty,
                    Email = user.Email ?? string.Empty
                }
            };
        }

        public async Task<bool> ValidateCredentialsAsync(string username, string password)
        {
            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null)
            {
                return false;
            }

            return await _userRepository.CheckPasswordAsync(user, password);
        }

        public async Task<AuthResponseDto> VerifyEmailAsync(string token)
        {
            var user = await _userRepository.GetByVerificationTokenAsync(token);
            if (user == null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Invalid verification token."
                };
            }

            if (user.IsEmailVerified)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Email is already verified."
                };
            }

            if (user.EmailVerificationTokenExpiry < DateTime.UtcNow)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Verification token has expired. Please request a new one."
                };
            }

            user.IsEmailVerified = true;
            user.EmailVerificationToken = null;
            user.EmailVerificationTokenExpiry = null;
            await _userRepository.UpdateAsync(user);

            var roles = await _userRepository.GetUserRolesAsync(user);
            var jwtToken = _tokenService.GenerateJwtToken(user, roles);

            return new AuthResponseDto
            {
                Success = true,
                Message = "Email verified successfully. You can now log in.",
                Token = jwtToken,
                User = new UserDto
                {
                    Id = user.Id,
                    Username = user.UserName ?? string.Empty,
                    Email = user.Email ?? string.Empty
                }
            };
        }
    }
}
