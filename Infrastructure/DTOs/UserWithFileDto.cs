namespace RealEstate.Application.DTOs
{
    public class UserWithFileDto : UserDto
    {
        public IFormFile? PhotoFile { get; set; } = null!;
    }
}
