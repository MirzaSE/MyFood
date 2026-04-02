using MyFood.Application.Dtos;
using MyFood.Application.Models;

namespace MyFood.Application.Services;

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
            return Failure("Invalid registration request.");
        }

        if (string.IsNullOrWhiteSpace(registerDto.Username) ||
            string.IsNullOrWhiteSpace(registerDto.Email) ||
            string.IsNullOrWhiteSpace(registerDto.Password))
        {
            return Failure("Username, email, and password are required.");
        }

        if (!_passwordService.IsPasswordStrong(registerDto.Password))
        {
            return Failure(_passwordService.GetPasswordValidationError(registerDto.Password));
        }

        var usernameExists = await _userRepository.GetByUsernameAsync(registerDto.Username.Trim());
        if (usernameExists != null)
        {
            return Failure("Username already exists.");
        }

        var emailExists = await _userRepository.GetByEmailAsync(registerDto.Email.Trim());
        if (emailExists != null)
        {
            return Failure("Email already exists.");
        }

        var user = new AuthUser
        {
            Username = registerDto.Username.Trim(),
            Email = registerDto.Email.Trim(),
            FullName = string.IsNullOrWhiteSpace(registerDto.FullName)
                ? registerDto.Username.Trim()
                : registerDto.FullName.Trim(),
            PasswordHash = _passwordService.HashPassword(registerDto.Password),
            EmailVerified = false,
            EmailVerificationToken = _verificationTokenService.GenerateToken(),
            EmailVerificationTokenExpiryUtc = DateTime.UtcNow.AddHours(24)
        };

        var created = await _userRepository.CreateAsync(user);
        await _emailService.SendVerificationEmailAsync(created.Email, created.EmailVerificationToken);

        return new AuthResponseDto
        {
            Success = true,
            Message = "User registered successfully. Please verify your email before logging in.",
            User = ToUserDto(created)
        };
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
    {
        if (loginDto == null ||
            string.IsNullOrWhiteSpace(loginDto.Username) ||
            string.IsNullOrWhiteSpace(loginDto.Password))
        {
            return Failure("Username and password are required.");
        }

        var user = await _userRepository.GetByUsernameAsync(loginDto.Username.Trim());
        if (user == null)
        {
            return Failure("Invalid username or password.");
        }

        if (!user.EmailVerified)
        {
            return Failure("Please verify your email before logging in.");
        }

        var isPasswordValid = _passwordService.VerifyPassword(loginDto.Password, user.PasswordHash);
        if (!isPasswordValid)
        {
            return Failure("Invalid username or password.");
        }

        var token = _tokenService.GenerateToken(user);

        return new AuthResponseDto
        {
            Success = true,
            Message = "Login successful.",
            Token = token,
            User = ToUserDto(user)
        };
    }

    public async Task<AuthResponseDto> VerifyEmailAsync(VerifyEmailDto verifyEmailDto)
    {
        if (verifyEmailDto == null ||
            string.IsNullOrWhiteSpace(verifyEmailDto.Email) ||
            string.IsNullOrWhiteSpace(verifyEmailDto.Token))
        {
            return Failure("Email and token are required.");
        }

        var user = await _userRepository.GetByEmailAsync(verifyEmailDto.Email.Trim());
        if (user == null)
        {
            return Failure("User not found.");
        }

        if (user.EmailVerified)
        {
            return new AuthResponseDto
            {
                Success = true,
                Message = "Email is already verified.",
                User = ToUserDto(user)
            };
        }

        if (string.IsNullOrWhiteSpace(user.EmailVerificationToken) || user.EmailVerificationTokenExpiryUtc == null)
        {
            return Failure("Verification token is missing.");
        }

        var isValid = _verificationTokenService.ValidateToken(
            verifyEmailDto.Token,
            user.EmailVerificationToken,
            user.EmailVerificationTokenExpiryUtc.Value);

        if (!isValid)
        {
            return Failure("Verification token is invalid or expired.");
        }

        user.EmailVerified = true;
        user.EmailVerificationToken = string.Empty;
        user.EmailVerificationTokenExpiryUtc = null;

        var updatedUser = await _userRepository.UpdateAsync(user);
        var token = _tokenService.GenerateToken(updatedUser);

        return new AuthResponseDto
        {
            Success = true,
            Message = "Email verified successfully.",
            Token = token,
            User = ToUserDto(updatedUser)
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

        return _passwordService.VerifyPassword(password, user.PasswordHash);
    }

    private static AuthResponseDto Failure(string message)
    {
        return new AuthResponseDto
        {
            Success = false,
            Message = message
        };
    }

    private static UserDto ToUserDto(AuthUser user)
    {
        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            FullName = user.FullName,
            EmailVerified = user.EmailVerified
        };
    }
}
