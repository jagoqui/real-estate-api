using System;
using MongoDB.Driver;
using RealEstate.Application.Contracts;
using RealEstate.Domain.Entities;

namespace RealEstate.Infrastructure.API.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<User> _users;

        public UserRepository(IMongoDatabase database)
        {
            _users = database.GetCollection<User>("Users");
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

        public async Task<User> CreateAsync(User user)
        {
            await _users.InsertOneAsync(user);
            return user;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _users.Find(_ => true).ToListAsync();
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
