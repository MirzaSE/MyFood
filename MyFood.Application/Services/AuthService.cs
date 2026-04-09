using MyFood.Application.Dtos;
using MyFood.Application.Repositories;

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
            return new AuthResponseDto { Success = false, Message = "Invalid registration data." };

        if (string.IsNullOrWhiteSpace(registerDto.Username) || string.IsNullOrWhiteSpace(registerDto.Password))
            return new AuthResponseDto { Success = false, Message = "Username and password are required." };

        if (!_passwordService.IsPasswordStrong(registerDto.Password))
        {
            var errorMessage = _passwordService.GetPasswordValidationError(registerDto.Password);
            return new AuthResponseDto { Success = false, Message = errorMessage };
        }

        var exists = await _userRepository.UserExistsAsync(registerDto.Username);
        if (exists)
            return new AuthResponseDto { Success = false, Message = "Username already exists." };

        var verificationToken = _verificationTokenService.GenerateToken();
        var tokenExpiry = DateTime.UtcNow.AddHours(24);

        var createResult = await _userRepository.CreateUserAsync(registerDto, verificationToken, tokenExpiry);
        if (!createResult.Success)
            return new AuthResponseDto { Success = false, Message = string.Join("; ", createResult.Errors) };

        var user = await _userRepository.GetByUsernameAsync(registerDto.Username);
        if (user == null)
            return new AuthResponseDto { Success = false, Message = "Failed to load created user." };

        await _emailService.SendVerificationEmailAsync(registerDto.Email ?? registerDto.Username, verificationToken);

        return new AuthResponseDto
        {
            Success = true,
            Message = "User registered successfully. Check your email for verification.",
            Token = null,
            User = user
        };
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
    {
        if (loginDto == null)
            return new AuthResponseDto { Success = false, Message = "Invalid login request." };

        if (string.IsNullOrWhiteSpace(loginDto.Username) || string.IsNullOrWhiteSpace(loginDto.Password))
            return new AuthResponseDto { Success = false, Message = "Username and password are required." };

        var user = await _userRepository.GetByUsernameAsync(loginDto.Username);
        if (user == null)
            return new AuthResponseDto { Success = false, Message = "Invalid username or password." };

        var isVerified = await _userRepository.IsEmailVerifiedAsync(loginDto.Username);
        if (!isVerified)
            return new AuthResponseDto { Success = false, Message = "Email not verified. Please verify your email before logging in." };

        var valid = await _userRepository.CheckPasswordAsync(loginDto.Username, loginDto.Password);
        if (!valid)
            return new AuthResponseDto { Success = false, Message = "Invalid username or password." };

        var token = _tokenService.GenerateToken(user);

        return new AuthResponseDto
        {
            Success = true,
            Message = "Login successful.",
            Token = token,
            User = user
        };
    }

    public async Task<AuthResponseDto> VerifyEmailAsync(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return new AuthResponseDto { Success = false, Message = "Verification token is required." };

        var result = await _userRepository.SetEmailVerificationAsync(token);
        if (!result.Success)
            return new AuthResponseDto { Success = false, Message = result.Error ?? "Email verification failed." };

        return new AuthResponseDto { Success = true, Message = "Email verified successfully." };
    }

    public async Task<bool> ValidateCredentialsAsync(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            return false;

        return await _userRepository.CheckPasswordAsync(username, password);
    }
}
