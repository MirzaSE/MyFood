using MyFood.Application.Dtos;
using MyFood.Application.Repositories;
using MyFood.Domain.Entities;

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
        if (string.IsNullOrWhiteSpace(registerDto.Username) ||
            string.IsNullOrWhiteSpace(registerDto.Email) ||
            string.IsNullOrWhiteSpace(registerDto.Password))
        {
            return new AuthResponseDto
            {
                Success = false,
                Message = "Username, email, and password are required."
            };
        }

        var existingUser = await _userRepository.GetByUsernameAsync(registerDto.Username);
        if (existingUser is not null)
        {
            return new AuthResponseDto
            {
                Success = false,
                Message = "A user with this username already exists."
            };
        }

        var existingEmail = await _userRepository.GetByEmailAsync(registerDto.Email);
        if (existingEmail is not null)
        {
            return new AuthResponseDto
            {
                Success = false,
                Message = "A user with this email already exists."
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
            NormalizedUserName = registerDto.Username.ToUpperInvariant(),
            Email = registerDto.Email,
            NormalizedEmail = registerDto.Email.ToUpperInvariant(),
            SecurityStamp = Guid.NewGuid().ToString(),
            ConcurrencyStamp = Guid.NewGuid().ToString(),
            PasswordHash = _passwordService.HashPassword(registerDto.Password),
            IsEmailVerified = false,
            VerificationToken = verificationToken,
            VerificationTokenExpiryTime = DateTime.UtcNow.AddHours(24)
        };

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();
        await _emailService.SendVerificationEmailAsync(user.Email!, verificationToken);

        return new AuthResponseDto
        {
            Success = true,
            Message = "User registered successfully. Please verify your email before logging in.",
            User = ToUserDto(user)
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
        if (user is null)
        {
            return new AuthResponseDto
            {
                Success = false,
                Message = "Invalid username or password."
            };
        }

        if (!_passwordService.VerifyPassword(loginDto.Password, user.PasswordHash ?? string.Empty))
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
                Message = "Email verification is required before logging in."
            };
        }

        return new AuthResponseDto
        {
            Success = true,
            Message = "Login successful.",
            Token = _tokenService.GenerateToken(user),
            User = ToUserDto(user)
        };
    }

    public async Task<bool> ValidateCredentialsAsync(string username, string password)
    {
        var user = await _userRepository.GetByUsernameAsync(username);
        return user is not null
            && user.IsEmailVerified
            && _passwordService.VerifyPassword(password, user.PasswordHash ?? string.Empty);
    }

    public async Task<AuthResponseDto> VerifyEmailAsync(VerifyEmailDto verifyEmailDto)
    {
        var user = await _userRepository.GetByUsernameAsync(verifyEmailDto.Username);
        if (user is null)
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
                User = ToUserDto(user)
            };
        }

        if (string.IsNullOrWhiteSpace(user.VerificationToken) || user.VerificationTokenExpiryTime is null)
        {
            return new AuthResponseDto
            {
                Success = false,
                Message = "No verification token is available for this user."
            };
        }

        var isValid = _verificationTokenService.ValidateToken(
            verifyEmailDto.Token,
            user.VerificationToken,
            user.VerificationTokenExpiryTime.Value);

        if (!isValid)
        {
            return new AuthResponseDto
            {
                Success = false,
                Message = "The verification token is invalid or has expired."
            };
        }

        user.IsEmailVerified = true;
        user.EmailConfirmed = true;
        user.VerificationToken = null;
        user.VerificationTokenExpiryTime = null;

        await _userRepository.UpdateAsync(user);
        await _userRepository.SaveChangesAsync();

        return new AuthResponseDto
        {
            Success = true,
            Message = "Email verified successfully.",
            User = ToUserDto(user),
            Token = _tokenService.GenerateToken(user)
        };
    }

    private static UserDto ToUserDto(ApplicationUser user)
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
