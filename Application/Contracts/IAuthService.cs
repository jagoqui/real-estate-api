using RealEstate.Application.DTOs;

namespace RealEstate.Application.Contracts
{
    public interface IAuthService
    {
        Task<(string accessToken, string refreshToken, UserDto user)> RegisterAsync(string email, string name, string password);
        Task<(string accessToken, string refreshToken, UserDto user)> LoginWithEmailAsync(string email, string password);
        Task<(string accessToken, string refreshToken)> RefreshTokenAsync(string refreshToken);
        Task<AuthResponseDto> LoginWithGoogleCodeAsync(string code);
    }
}
