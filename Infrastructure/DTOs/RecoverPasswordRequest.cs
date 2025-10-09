namespace RealEstate.Infrastructure.DTOs
{
    public class ChangeUserPasswordRequest
    {
        public string UserId { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
    }
}