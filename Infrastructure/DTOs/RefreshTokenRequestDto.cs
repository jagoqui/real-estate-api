namespace RealEstate.Application.DTOs
{
    public class RefreshTokenDto
    {
        public string RefreshToken { get; set; } = null!;
    }

    public class RefreshTokenRequestDto : RefreshTokenDto
    {
        public string Token { get; set; } = null!;
    }
}
