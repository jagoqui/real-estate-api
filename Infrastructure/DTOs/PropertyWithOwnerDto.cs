namespace RealEstate.Infrastructure.DTOs
{
    public class PropertyWithOwnerDto
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } = null!;

        // Datos del dueño
        public string OwnerId { get; set; } = null!;
        public string OwnerName { get; set; } = null!;
        public string OwnerEmail { get; set; } = null!;
        public string OwnerPhone { get; set; } = null!;
    }
}
