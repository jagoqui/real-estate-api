using RealEstate.Domain.Enums;

namespace RealEstate.Application.DTOs
{
    public class UserBaseDto
    {
        public string Email { get; set; } = null!;
        public string? Name { get; set; }
        public UserRole Role { get; set; } = UserRole.OWNER;
    }

    public class UserDto : UserBaseDto
    {
        public string Id { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public string? Bio { get; set; }
        public string? PhotoUrl { get; set; }
        public string? GoogleId { get; set; }
    }

    public class UserCreateDto : UserBaseDto
    {
        public string Password { get; set; } = null!;
    }
}
