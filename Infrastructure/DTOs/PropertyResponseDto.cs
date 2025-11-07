using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using RealEstate.Domain.Entities;

namespace RealEstate.Infrastructure.DTOs
{
    public class PropertyResponseDto
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public decimal Price { get; set; }
        public string CodeInternal { get; set; } = null!;
        public int Year { get; set; }
        public string Description { get; set; } = null!;
        public int Bathrooms { get; set; }
        public int Bedrooms { get; set; }
        public int AreaSqm { get; set; }
        public List<string> HighlightedFeatures { get; set; } = new();
        public List<AmenityDto> Amenities { get; set; } = new();
        public bool Featured { get; set; } = false;
        public List<string> Images { get; set; } = new();
        public string? CoverImage { get; set; }
        public List<string> Views360Url { get; set; } = new();
        public string City { get; set; } = null!;
        public string State { get; set; } = null!;
        public string Country { get; set; } = null!;
        public Location Location { get; set; } = null!;
        public string IdOwner { get; set; } = null!;
        public string Status { get; set; } = null!;
        public string Type { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
