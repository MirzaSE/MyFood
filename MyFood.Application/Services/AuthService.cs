using MyFood.Application.Dtos;
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
        var passwordError = _passwordService.GetPasswordValidationError(registerDto.Password);
        if (!string.IsNullOrEmpty(passwordError))
            return new AuthResponseDto { Success = false, Message = passwordError };

        var userExists = await _userRepository.FindByUsernameAsync(registerDto.Username);
        if (userExists != null)
            return new AuthResponseDto { Success = false, Message = "Username is already taken." };

        var emailExists = await _userRepository.FindByEmailAsync(registerDto.Email);
        if (emailExists != null)
            return new AuthResponseDto { Success = false, Message = "Email is already registered." };
        var verificationToken = _verificationTokenService.GenerateToken();
        var tokenExpiry = _verificationTokenService.GetTokenExpiry();

        var user = new ApplicationUser
        {
            UserName = registerDto.Username,
            Email = registerDto.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            EmailConfirmed = false,
            EmailVerificationToken = verificationToken,
            EmailVerificationTokenExpiry = tokenExpiry
        };

        var (succeeded, errors) = await _userRepository.CreateAsync(user, registerDto.Password);
        if (!succeeded)
        {
            return new AuthResponseDto
            {
                Success = false,
                Message = "Registration failed: " + string.Join(", ", errors)
            };
        }
        await _emailService.SendVerificationEmailAsync(registerDto.Email, verificationToken);

        return new AuthResponseDto
        {
            Success = true,
            Message = "Registration successful. Please check your email to verify your account.",
            User = new UserDto { Username = user.UserName! }
        };
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
    {
        if (string.IsNullOrWhiteSpace(loginDto.Username) || string.IsNullOrWhiteSpace(loginDto.Password))
            return new AuthResponseDto { Success = false, Message = "Username and password are required." };

        var user = await _userRepository.FindByUsernameAsync(loginDto.Username);
        if (user == null)
            return new AuthResponseDto { Success = false, Message = "Invalid credentials." };

        var passwordValid = await _userRepository.CheckPasswordAsync(user, loginDto.Password);
        if (!passwordValid)
            return new AuthResponseDto { Success = false, Message = "Invalid credentials." };

        if (!user.EmailConfirmed)
            return new AuthResponseDto { Success = false, Message = "Please verify your email address before logging in." };

        var roles = await _userRepository.GetRolesAsync(user);
        var token = _tokenService.GenerateToken(user, roles);

        return new AuthResponseDto
        {
            Success = true,
            Message = "Login successful.",
            Token = token,
            User = new UserDto { Username = user.UserName! }
        };
    }
    public async Task<AuthResponseDto> VerifyEmailAsync(string email, string token)
    {
        var user = await _userRepository.FindByEmailAsync(email);
        if (user == null)
            return new AuthResponseDto { Success = false, Message = "User not found." };

        if (user.EmailConfirmed)
            return new AuthResponseDto { Success = true, Message = "Email is already verified." };

        if (user.EmailVerificationToken == null || user.EmailVerificationTokenExpiry == null)
            return new AuthResponseDto { Success = false, Message = "No pending verification found." };

        if (!_verificationTokenService.ValidateToken(token, user.EmailVerificationToken, user.EmailVerificationTokenExpiry.Value))
            return new AuthResponseDto { Success = false, Message = "Verification token is invalid or has expired." };
        
        user.EmailConfirmed = true;
        user.EmailVerificationToken = null;
        user.EmailVerificationTokenExpiry = null;

        var (succeeded, errors) = await _userRepository.UpdateAsync(user);
        if (!succeeded)
        {
            return new AuthResponseDto
            {
                Success = false,
                Message = "Could not save verification: " + string.Join(", ", errors)
            };
        }

        return new AuthResponseDto
        {
            Success = true,
            Message = "Email verified successfully. You can now log in.",
            User = new UserDto { Username = user.UserName! }
        };
    }

    public async Task<bool> ValidateCredentialsAsync(string username, string password)
    {
        var user = await _userRepository.FindByUsernameAsync(username);
        if (user == null)
            return false;

        return await _userRepository.CheckPasswordAsync(user, password);
    }
}
