using MongoDB.Driver;
using RealEstate.Application.Contracts;
using RealEstate.Application.DTOs;
using RealEstate.Domain.Entities;

namespace RealEstate.Infrastructure.API.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<User> _users;
        private readonly IOwnerRepository _ownerRepository;

        public UserRepository(IMongoDatabase database, IOwnerRepository ownerRepository)
        {
            _users = database.GetCollection<User>("Users");
            _ownerRepository = ownerRepository;
        }

        public async Task<User> CreateAsync(User user)
        {
            await _users.InsertOneAsync(user);
            return user;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _users.Find(u => u.Email == email).FirstOrDefaultAsync();
        }

        public async Task<User?> GetByGoogleIdAsync(string googleId)
        {
            return await _users.Find(u => u.GoogleId == googleId).FirstOrDefaultAsync();
        }

        public async Task<User?> GetByIdAsync(string id)
        {
            return await _users.Find(u => u.Id == id).FirstOrDefaultAsync();
        }

        public async Task<User?> UpdateAsync(UserDto user)
        {
            var update = Builders<User>.Update
                .Set(u => u.Name, user.Name)
                .Set(u => u.PhotoUrl, user.PhotoUrl)
                .Set(u => u.PhoneNumber, user.PhoneNumber)
                .Set(u => u.Bio, user.Bio);

            if (user.Id == null)
            {
                return null;
            }

            await _users.UpdateOneAsync(u => u.Id == user.Id, update);
            return await GetByIdAsync(user.Id);
        }

        public async Task<IEnumerable<User>> GetUserWithoutOwnersAsync()
        {
            var owners = await _ownerRepository.GetOwnersAsync();
            var ownerUserIds = owners.Where(o => !string.IsNullOrEmpty(o.UserId)).Select(o => o.UserId).ToHashSet();

            var filter = Builders<User>.Filter.Nin(u => u.Id, ownerUserIds);
            return await _users.Find(filter).ToListAsync();
        }

        public async Task<User?> RecoverAsync(string userId, string email, string newPasswordHash)
        {
            var update = Builders<User>.Update
                .Set(u => u.PasswordHash, newPasswordHash);

            var filter = Builders<User>.Filter.Eq(u => u.Id, userId) & Builders<User>.Filter.Eq(u => u.Email, email);

            var result = await _users.UpdateOneAsync(filter, update);
            if (result.ModifiedCount == 0)
            {
                return null;
            }

            return await GetByIdAsync(userId);
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _users.Find(_ => true).ToListAsync();
        }

        public async Task DeleteAsync(string userId)
        {
            await _users.DeleteOneAsync(u => u.Id == userId);
        }

        public async Task SaveRefreshTokenAsync(string userId, string refreshToken, DateTime? expiryTime = null)
        {
            var update = Builders<User>.Update
                .Set(u => u.RefreshToken, refreshToken)
                .Set(u => u.RefreshTokenExpiryTime, expiryTime ?? DateTime.UtcNow.AddDays(7));

            await _users.UpdateOneAsync(u => u.Id == userId, update);
        }

        public async Task<User?> GetByRefreshTokenAsync(string refreshToken)
        {
            return await _users.Find(u => u.RefreshToken == refreshToken).FirstOrDefaultAsync();
        }

        public async Task<bool> ReplaceRefreshTokenAsync(string currentRefreshToken, string newRefreshToken, DateTime? expiryTime = null)
        {
            var filter = Builders<User>.Filter.Eq(u => u.RefreshToken, currentRefreshToken);
            var update = Builders<User>.Update
                .Set(u => u.RefreshToken, newRefreshToken)
                .Set(u => u.RefreshTokenExpiryTime, expiryTime ?? DateTime.UtcNow.AddDays(7));

            var result = await _users.UpdateOneAsync(filter, update);
            return result.ModifiedCount == 1;
        }
    }
}
