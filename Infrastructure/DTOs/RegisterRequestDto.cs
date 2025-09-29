namespace RealEstate.Application.DTOs
{
    public class RegisterRequestDto
    {
        public string Email { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
