namespace RealEstate.Application.DTOs
{
    public class RefreshTokenRequestDto
    {
        public string Token { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
    }
}
