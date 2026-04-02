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

        public AuthService(
            IUserRepository userRepository,
            ITokenService tokenService,
            IPasswordService passwordService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _passwordService = passwordService;
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

            var user = new ApplicationUser
            {
                UserName = registerDto.Username,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var result = await _userRepository.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                return FailureResponse(result.Errors.Select(e => e.Description));
            }

            return await GenerateAuthResponseAsync(user);
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

            return await GenerateAuthResponseAsync(user);
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
    }
}
