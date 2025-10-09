using RealEstate.Application.DTOs;
using RealEstate.Domain.Entities;
using RealEstate.Infrastructure.DTOs;

namespace RealEstate.Application.Services
{
    public interface IUserService
    {
        Task<UserDto> CreateUserAsync(UserCreateDto request);
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<UserDto> GetUserByIdAsync(string userId);
        Task<UserDto> UpdateAsync(string id, UserWithFileDto user);
        Task<UserDto> RecoverPasswordAsync(RecoverPasswordRequest request);
        Task<bool> DeleteUserAsync(string userId);
    }
}
