using RealEstate.Domain.Entities;

namespace RealEstate.Application.Contracts
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByGoogleIdAsync(string googleId);
        Task<User?> GetByIdAsync(string id); // Obtener usuario por ID
        Task<User> CreateAsync(User user);
        Task<IEnumerable<User>> GetAllAsync();

        // =======================
        // Refresh token
        // =======================
        Task SaveRefreshTokenAsync(string userId, string refreshToken, DateTime? expiryTime = null);
        Task<User?> GetByRefreshTokenAsync(string refreshToken);
    }
}
