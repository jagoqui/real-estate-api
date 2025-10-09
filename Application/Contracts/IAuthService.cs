using RealEstate.Application.DTOs;

namespace RealEstate.Application.Contracts
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(UserCreateDto request);
        Task<AuthResponseDto> LoginWithEmailAsync(string email, string password);
        Task<AuthResponseDto> RefreshTokenAsync(string refreshToken);
        Task<AuthResponseDto> LoginWithGoogleCodeAsync(string code);
        Task LogoutAsync();
    }
}
