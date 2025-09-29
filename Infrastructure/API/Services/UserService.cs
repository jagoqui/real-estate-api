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
    }
}
