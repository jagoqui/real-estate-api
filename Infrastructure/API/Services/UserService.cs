using RealEstate.Application.Contracts;
using RealEstate.Application.DTOs;
using RealEstate.Domain.Entities;

namespace RealEstate.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;

        public UserService(IUserRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _repository.GetAllAsync();

            return users.Select(u => new UserDto
            {
                Id = u.Id!,
                Email = u.Email,
                Name = u.Name,
                GoogleId = u.GoogleId,
                Role = u.Role,
            });
        }

        public async Task<UserDto> GetUserByIdAsync(string userId)
        {
            var user = await _repository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {userId} not found.");
            }

            return new UserDto
            {
                Id = user.Id!,
                Email = user.Email,
                Name = user.Name,
                GoogleId = user.GoogleId,
                Role = user.Role,
            };
        }

        public async Task<UserDto> UpdateAsync(string id, UserDto user)
        {
            if (id != user.Id)
            {
                throw new ArgumentException("User ID mismatch.");
            }
            var updatedUser = await _repository.UpdateAsync(user);
            if (updatedUser == null)
            {
                throw new KeyNotFoundException($"User with ID {user.Id} not found for update.");
            }

            return new UserDto
            {
                Id = updatedUser.Id!,
                Email = updatedUser.Email,
                Name = updatedUser.Name,
                GoogleId = updatedUser.GoogleId,
                Role = updatedUser.Role,
            };
        }

        public async Task<bool> DeleteUserAsync(string userId)
        {
            var user = await _repository.GetByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            await _repository.DeleteAsync(userId);
            return true;
        }
    }
}
