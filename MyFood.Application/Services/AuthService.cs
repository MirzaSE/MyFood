using MyFood.Application.Dtos;
using MyFood.Domain.Entities;

namespace MyFood.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly IPasswordService _passwordService;

    public AuthService(IUserRepository userRepository, ITokenService tokenService, IPasswordService passwordService)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _passwordService = passwordService;
    }

    public async Task<AuthResponseDto?> RegisterAsync(RegisterDto registerDto)
    {
        var passwordError = _passwordService.GetPasswordValidationError(registerDto.Password);
        if (passwordError != null)
            throw new ArgumentException(passwordError);

        var userExists = await _userRepository.FindByUsernameAsync(registerDto.Username);
        if (userExists != null)
            return null;

        var user = new ApplicationUser
        {
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = registerDto.Username
        };

        var created = await _userRepository.CreateAsync(user, registerDto.Password);
        if (!created)
            return null;

        var roles = await _userRepository.GetRolesAsync(user);
        var (token, expiration) = await _tokenService.GenerateTokenAsync(user, roles);
        return new AuthResponseDto
        {
            Token = token,
            Expiration = expiration,
            User = new UserDto { Username = user.UserName! }
        };
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginDto loginDto)
    {
        var user = await _userRepository.FindByUsernameAsync(loginDto.Username);
        if (user == null || !await _userRepository.CheckPasswordAsync(user, loginDto.Password))
            return null;

        var roles = await _userRepository.GetRolesAsync(user);
        var (token, expiration) = await _tokenService.GenerateTokenAsync(user, roles);
        return new AuthResponseDto
        {
            Token = token,
            Expiration = expiration,
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
