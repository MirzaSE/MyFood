using MyFood.Application.DTOs.Auth;
using MyFood.Application.Dtos;
using MyFood.Domain.Entities;

namespace MyFood.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordService _passwordService;
        private readonly ITokenService _tokenService;

        public AuthService(
            IUserRepository userRepository,
            IPasswordService passwordService,
            ITokenService tokenService)
        {
            _userRepository = userRepository;
            _passwordService = passwordService;
            _tokenService = tokenService;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterUserDto dto)
        {
            var existing = await _userRepository.GetByUsernameAsync(dto.Username);

            if (existing != null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "User exists"
                };
            }

            var user = new ApplicationUser
            {
                UserName = dto.Username,
                PasswordHash = _passwordService.HashPassword(dto.Password)
            };

            await _userRepository.AddAsync(user);

            var token = _tokenService.GenerateToken(user);

            return new AuthResponseDto
            {
                Success = true,
                Token = token
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await _userRepository.GetByUsernameAsync(dto.Username);

            if (user == null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "User not found"
                };
            }

            var valid = _passwordService.VerifyPassword(dto.Password, user.PasswordHash);

            if (!valid)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Wrong password"
                };
            }

            var token = _tokenService.GenerateToken(user);

            return new AuthResponseDto
            {
                Success = true,
                Token = token
            };
        }
    }
}