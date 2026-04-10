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
            if (string.IsNullOrWhiteSpace(registerDto.Username) ||
                string.IsNullOrWhiteSpace(registerDto.Email) ||
                string.IsNullOrWhiteSpace(registerDto.Password))
            {
                return new AuthResponseDto { Success = false, Message = "Username, email, and password are required." };
            }

            var existingByUsername = await _userRepository.FindByUsernameAsync(registerDto.Username);
            if (existingByUsername != null)
            {
                return new AuthResponseDto { Success = false, Message = "Username already exists." };
            }

            var existingByEmail = await _userRepository.FindByEmailAsync(registerDto.Email);
            if (existingByEmail != null)
            {
                return new AuthResponseDto { Success = false, Message = "Email already exists." };
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
            var newUser = new ApplicationUser
            {
                UserName = registerDto.Username,
                Email = registerDto.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                IsEmailVerified = false,
                VerificationToken = verificationToken,
                VerificationTokenExpiryUtc = DateTime.UtcNow.AddHours(24)
            };

            var created = await _userRepository.CreateAsync(newUser, registerDto.Password);
            if (!created)
            {
                return new AuthResponseDto { Success = false, Message = "User creation failed." };
            }

            await _emailService.SendVerificationEmailAsync(newUser.Email!, verificationToken);
            var token = await _tokenService.GenerateTokenAsync(newUser);

            return new AuthResponseDto
            {
                Success = true,
                Message = "Registration successful.",
                Token = token,
                User = MapUser(newUser)
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            if (string.IsNullOrWhiteSpace(loginDto.Username) || string.IsNullOrWhiteSpace(loginDto.Password))
            {
                return new AuthResponseDto { Success = false, Message = "Username and password are required." };
            }

            var user = await _userRepository.FindByUsernameAsync(loginDto.Username);
            if (user == null)
            {
                return new AuthResponseDto { Success = false, Message = "Invalid username or password." };
            }

            var isPasswordValid = await ValidateCredentialsAsync(loginDto.Username, loginDto.Password);
            if (!isPasswordValid)
            {
                return new AuthResponseDto { Success = false, Message = "Invalid username or password." };
            }

            if (!user.IsEmailVerified)
            {
                return new AuthResponseDto { Success = false, Message = "Email is not verified." };
            }

            var token = await _tokenService.GenerateTokenAsync(user);
            return new AuthResponseDto
            {
                Success = true,
                Message = "Login successful.",
                Token = token,
                User = MapUser(user)
            };
        }

        public async Task<bool> ValidateCredentialsAsync(string username, string password)
        {
            var user = await _userRepository.FindByUsernameAsync(username);
            if (user == null)
            {
                return false;
            }

            var passwordHash = user.PasswordHash ?? string.Empty;
            if (string.IsNullOrWhiteSpace(passwordHash))
            {
                return await _userRepository.CheckPasswordAsync(user, password);
            }

            var isValid = _passwordService.VerifyPassword(password, passwordHash);
            if (isValid)
            {
                return true;
            }

            return await _userRepository.CheckPasswordAsync(user, password);
        }

        public async Task<AuthResponseDto> VerifyEmailAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return new AuthResponseDto { Success = false, Message = "Verification token is required." };
            }

            var user = await _userRepository.FindByVerificationTokenAsync(token);

            if (user == null)
            {
                var userIdFromJwt = _tokenService.GetUserIdFromToken(token);
                if (string.IsNullOrWhiteSpace(userIdFromJwt))
                {
                    return new AuthResponseDto { Success = false, Message = "Invalid verification token." };
                }

                user = await _userRepository.FindByIdAsync(userIdFromJwt);
                if (user == null)
                {
                    return new AuthResponseDto { Success = false, Message = "Invalid verification token." };
                }

                if (user.IsEmailVerified)
                {
                    return new AuthResponseDto
                    {
                        Success = true,
                        Message = "Email is already verified.",
                        Token = await _tokenService.GenerateTokenAsync(user),
                        User = MapUser(user)
                    };
                }

                user.IsEmailVerified = true;
                user.VerificationToken = null;
                user.VerificationTokenExpiryUtc = null;

                var updatedFromJwt = await _userRepository.UpdateAsync(user);
                if (!updatedFromJwt)
                {
                    return new AuthResponseDto { Success = false, Message = "Unable to verify email." };
                }

                var jwtTokenFromJwt = await _tokenService.GenerateTokenAsync(user);
                return new AuthResponseDto
                {
                    Success = true,
                    Message = "Email verified successfully.",
                    Token = jwtTokenFromJwt,
                    User = MapUser(user)
                };
            }

            if (string.IsNullOrWhiteSpace(user.VerificationToken) || !user.VerificationTokenExpiryUtc.HasValue)
            {
                return new AuthResponseDto { Success = false, Message = "Invalid verification token." };
            }

            var isValidToken = _verificationTokenService.ValidateToken(
                token,
                user.VerificationToken,
                user.VerificationTokenExpiryUtc.Value);

            if (!isValidToken)
            {
                return new AuthResponseDto { Success = false, Message = "Verification token is invalid or expired." };
            }

            user.IsEmailVerified = true;
            user.VerificationToken = null;
            user.VerificationTokenExpiryUtc = null;
            var updated = await _userRepository.UpdateAsync(user);

            if (!updated)
            {
                return new AuthResponseDto { Success = false, Message = "Unable to verify email." };
            }

            var jwtToken = await _tokenService.GenerateTokenAsync(user);
            return new AuthResponseDto
            {
                Success = true,
                Message = "Email verified successfully.",
                Token = jwtToken,
                User = MapUser(user)
            };
        }

        private static UserDto MapUser(ApplicationUser user)
        {
            return new UserDto
            {
                Id = user.Id,
                Username = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                IsEmailVerified = user.IsEmailVerified
            };
        }
    }
}
