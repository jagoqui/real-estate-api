namespace RealEstate.Application.DTOs
{
    public class GoogleLoginRequestDto
    {
        public string Email { get; set; } = null!;
        public string GoogleId { get; set; } = null!;
        public string? Name { get; set; }
    }
}
