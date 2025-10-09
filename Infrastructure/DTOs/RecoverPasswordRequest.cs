namespace RealEstate.Infrastructure.DTOs
{
    public class RecoverPasswordRequest
    {
        public string UserId { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
    }
}