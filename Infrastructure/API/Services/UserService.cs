using RealEstate.Application.Contracts;
using RealEstate.Application.DTOs;

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

        public async Task<bool> DeleteUserAsync(string userId)
        {
            var user = await _repository.GetByIdAsync(userId);
            if (user == null)
            {
                return false; // User not found
            }

            await _repository.DeleteAsync(userId);
            return true; // User deleted successfully
        }
    }
}
