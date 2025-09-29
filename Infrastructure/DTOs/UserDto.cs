using RealEstate.Domain.Enums;

namespace RealEstate.Application.DTOs
{
    public class UserDto
    {
        public string Id { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Name { get; set; }
        public string? GoogleId { get; set; }
        public UserRole Role { get; set; }
    }
}
