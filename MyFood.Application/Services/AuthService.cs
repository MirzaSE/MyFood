using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MyFood.Application.Dtos;
using MyFood.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MyFood.Application.Services;

public class AuthService(
	UserManager<ApplicationUser> userManager,
	IConfiguration configuration,
	IPasswordService passwordService,
	IVerificationTokenService verificationTokenService) : IAuthService
{
	private readonly UserManager<ApplicationUser> _userManager = userManager;
	private readonly IConfiguration _configuration = configuration;
	private readonly IPasswordService _passwordService = passwordService;
	private readonly IVerificationTokenService _verificationTokenService = verificationTokenService;

	public async Task<AuthResponseDto> RegisterAsync(RegisterUserDto registerDto)
	{
		var username = registerDto.FullName?.Trim();
		var email = registerDto.Email?.Trim();

		if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(registerDto.Password))
		{
			return Failure("FullName, Email, and Password are required.", 400);
		}

		if (!_passwordService.IsPasswordStrong(registerDto.Password, out var strengthError))
		{
			return Failure(strengthError, 400);
		}

		var userByName = await _userManager.FindByNameAsync(username);
		if (userByName is not null)
		{
			return Failure("User already exists.", 409);
		}

		var userByEmail = await _userManager.FindByEmailAsync(email);
		if (userByEmail is not null)
		{
			return Failure("Email address is already in use.", 409);
		}

		var user = new ApplicationUser
		{
			FullName = username,
			UserName = username,
			Email = email,
			EmailConfirmed = true,
			IsEmailVerified = true,
			VerificationToken = null,
			VerificationTokenExpiry = null,
			PasswordHash = _passwordService.HashPassword(registerDto.Password)
		};

		var createResult = await _userManager.CreateAsync(user);
		if (!createResult.Succeeded)
		{
			return Failure($"Registration failed: {string.Join("; ", createResult.Errors.Select(e => e.Description))}", 400);
		}

		return new AuthResponseDto
		{
			Success = true,
			StatusCode = 201,
			Message = "Registration successful.",
			User = MapUser(user)
		};
	}

	public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
	{
		var username = loginDto.FullName?.Trim();

		if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(loginDto.Password))
		{
			return Failure("FullName and Password are required.", 400);
		}

		var user = await _userManager.FindByNameAsync(username);
		if (user is null)
		{
			return Failure("Invalid credentials.", 401);
		}

		var isValid = await ValidateCredentialsAsync(username, loginDto.Password);
		if (!isValid)
		{
			return Failure("Invalid credentials.", 401);
		}

		return new AuthResponseDto
		{
			Success = true,
			StatusCode = 200,
			Message = "Login successful.",
			Token = GenerateJwtToken(user),
			User = MapUser(user)
		};
	}

	public async Task<bool> ValidateCredentialsAsync(string username, string password)
	{
		if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
		{
			return false;
		}

		var user = await _userManager.FindByNameAsync(username);
		if (user is null || string.IsNullOrWhiteSpace(user.PasswordHash))
		{
			return false;
		}

		return _passwordService.VerifyPassword(password, user.PasswordHash);
	}

	public async Task<AuthResponseDto> VerifyEmailAsync(string emailOrUsername, string verificationToken)
	{
		if (string.IsNullOrWhiteSpace(emailOrUsername) || string.IsNullOrWhiteSpace(verificationToken))
		{
			return Failure("Email/username and verification token are required.", 400);
		}

		var user = await _userManager.FindByEmailAsync(emailOrUsername);
		user ??= await _userManager.FindByNameAsync(emailOrUsername);

		if (user is null)
		{
			return Failure("User was not found.", 404);
		}

		if (user.IsEmailVerified && user.EmailConfirmed)
		{
			return new AuthResponseDto
			{
				Success = true,
				StatusCode = 200,
				Message = "Email is already verified.",
				User = MapUser(user)
			};
		}

		if (!_verificationTokenService.ValidateToken(
			verificationToken,
			user.VerificationToken ?? string.Empty,
			user.VerificationTokenExpiry ?? DateTime.MinValue))
		{
			return Failure("Verification token is invalid or expired.", 400);
		}

		user.IsEmailVerified = true;
		user.EmailConfirmed = true;
		user.VerificationToken = null;
		user.VerificationTokenExpiry = null;

		var updateResult = await _userManager.UpdateAsync(user);
		if (!updateResult.Succeeded)
		{
			return Failure($"Email verification failed: {string.Join("; ", updateResult.Errors.Select(e => e.Description))}", 400);
		}

		return new AuthResponseDto
		{
			Success = true,
			StatusCode = 200,
			Message = "Email verified successfully.",
			User = MapUser(user)
		};
	}

	private string GenerateJwtToken(ApplicationUser user)
	{
		var claims = new[]
		{
			new Claim("UserId", user.Id),
			new Claim("Username", user.UserName ?? string.Empty),
			new Claim(ClaimTypes.NameIdentifier, user.Id),
			new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
			new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName ?? string.Empty)
		};

		var jwtKey = _configuration["Jwt:Key"];
		var issuer = _configuration["Jwt:Issuer"];
		var audience = _configuration["Jwt:Audience"];
		var durationRaw = _configuration["Jwt:DurationInMinutes"];

		var durationInMinutes = 60d;
		if (!string.IsNullOrWhiteSpace(durationRaw) && double.TryParse(durationRaw, out var configuredDuration))
		{
			durationInMinutes = configuredDuration;
		}

		var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey ?? throw new InvalidOperationException("Jwt:Key is not configured.")));
		var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

		var token = new JwtSecurityToken(
			issuer: issuer,
			audience: audience,
			claims: claims,
			expires: DateTime.UtcNow.AddMinutes(durationInMinutes),
			signingCredentials: creds
		);

		return new JwtSecurityTokenHandler().WriteToken(token);
	}

	private static UserDto MapUser(ApplicationUser user)
	{
		return new UserDto
		{
			Id = user.Id,
			Username = user.UserName,
			Email = user.Email,
			IsEmailVerified = user.IsEmailVerified
		};
	}

	private static AuthResponseDto Failure(string message, int statusCode)
	{
		return new AuthResponseDto
		{
			Success = false,
			Message = message,
			StatusCode = statusCode
		};
	}
}
