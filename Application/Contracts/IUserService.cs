using RealEstate.Application.DTOs;
using RealEstate.Infrastructure.DTOs;

namespace RealEstate.Application.Services
{
    public interface IUserService
    {
        Task<UserDto> CreateUserAsync(UserCreateDto request);
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<UserDto> GetUserByIdAsync(string userId);
        Task<IEnumerable<UserDto>> GetUsersWithoutOwnersAsync();
        Task<UserDto> UpdateAsync(string id, UserWithFileDto user);
        Task<UserDto> RecoverPasswordAsync(RecoverPasswordRequest request);
        Task<bool> DeleteUserAsync(string userId);
    }
}
