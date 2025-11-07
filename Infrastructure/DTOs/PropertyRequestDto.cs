using RealEstate.Domain.Entities;
using RealEstate.Domain.Enums;

namespace RealEstate.Infrastructure.DTOs
{
    public class PropertyRequestDto
    {
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public decimal Price { get; set; }
        public int Year { get; set; }
        public string Description { get; set; } = null!;
        public int Bathrooms { get; set; }
        public int Bedrooms { get; set; }
        public int AreaSqm { get; set; }
        public List<string> HighlightedFeatures { get; set; } = new();
        public List<AmenityDto> Amenities { get; set; } = new();
        public bool Featured { get; set; } = false;
        public List<string> Views360Url { get; set; } = new();
        public string City { get; set; } = null!;
        public string State { get; set; } = null!;
        public string Country { get; set; } = null!;
        public Location Location { get; set; } = null!;
        public string IdOwner { get; set; } = null!;
        public string Status { get; set; } = nameof(PropertyStatus.AVAILABLE);
        public string Type { get; set; } = nameof(PropertyTypes.OTHER);
        public List<IFormFile>? Images { get; set; }
    }
}
