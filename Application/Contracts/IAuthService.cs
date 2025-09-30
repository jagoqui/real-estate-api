using RealEstate.Application.DTOs;
using RealEstate.Infrastructure.DTOs;

namespace RealEstate.Application.Contracts
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(string email, string name, string password);
        Task<AuthResponseDto> LoginWithEmailAsync(string email, string password);
        Task<TokenDto> RefreshTokenAsync(string refreshToken);
        Task<AuthResponseDto> LoginWithGoogleCodeAsync(string code);
    }
}
