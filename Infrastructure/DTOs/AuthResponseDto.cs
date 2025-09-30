using RealEstate.Infrastructure.DTOs;

namespace RealEstate.Application.DTOs
{
    public class AuthResponseDto : TokenDto
    {
        public UserDto User { get; set; } = null!;
    }
}