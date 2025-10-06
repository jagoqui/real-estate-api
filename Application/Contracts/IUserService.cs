using RealEstate.Application.DTOs;

namespace RealEstate.Application.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<UserDto> GetUserByIdAsync(string userId);
        Task<UserDto> UpdateAsync(string id, UserWithFileDto user);
        Task<bool> DeleteUserAsync(string userId);
    }
}
