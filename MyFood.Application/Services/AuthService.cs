using Microsoft.AspNetCore.Identity;
using MyFood.Application.Dtos;
using MyFood.Application.Dtos;
using MyFood.Domain.Entities;
using LoginDto = MyFood.Application.Dtos.LoginDto;
using RegisterUserDto = MyFood.Application.Dtos.RegisterUserDto;

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
                Email = dto.Username + "@test.com",
                FullName = dto.Username 
            };

            var hasher = new PasswordHasher<ApplicationUser>();
            user.PasswordHash = hasher.HashPassword(user, dto.Password);

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

            var hasher = new PasswordHasher<ApplicationUser>();
            var result = hasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);

            if (result != PasswordVerificationResult.Success)
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