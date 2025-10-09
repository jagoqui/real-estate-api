using RealEstate.Application.DTOs;
using RealEstate.Domain.Entities;

namespace RealEstate.Application.Contracts
{
    public interface IUserRepository
    {
        Task<User> CreateAsync(User user);
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByGoogleIdAsync(string googleId);
        Task<User?> GetByIdAsync(string id);
        Task<IEnumerable<User>> GetAllAsync();
        Task<IEnumerable<User>> GetUserWithoutOwnersAsync();
        Task<User?> UpdateAsync(UserDto user);

        Task<User?> RecoverAsync(string userId, string email, string newPasswordHash);
        Task DeleteAsync(string userId);

        // =======================
        // Refresh token
        // =======================
        Task SaveRefreshTokenAsync(string userId, string refreshToken, DateTime? expiryTime = null);
        Task<User?> GetByRefreshTokenAsync(string refreshToken);

        Task<bool> ReplaceRefreshTokenAsync(string currentRefreshToken, string newRefreshToken, DateTime? expiryTime = null);
    }
}
