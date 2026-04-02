using MyFood.Application.Dtos;

namespace MyFood.Application.Services
{
    public interface IUserRepository
    {
        Task<UserDto?> GetByEmailAsync(string email);
        Task<UserDto?> GetByIdAsync(string id);
        Task<bool> ExistsByEmailAsync(string email);
        Task<UserDto> CreateAsync(UserDto user, string passwordHash);

        /// <summary>Finds user by email, username, or full name and returns the stored password hash for verification.</summary>
        Task<(UserDto User, string PasswordHash)?> FindForLoginAsync(string emailOrUsername);
        // Add more as needed
    }
}