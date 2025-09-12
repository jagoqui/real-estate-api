namespace RealEstate.Infrastructure.DTOs
{
    public class PropertyResponseDto
    {
        public string Id { get; set; } = null!;
        public string IdOwner { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } = null!;
    }
}
