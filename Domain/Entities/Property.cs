using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using RealEstate.Domain.Enums;

namespace RealEstate.Domain.Entities
{
    public abstract class PropertyBase
    {
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public decimal Price { get; set; }
        public int Year { get; set; }
        public string Description { get; set; } = null!;
        public int Bathrooms { get; set; }
        public int Bedrooms { get; set; }
        public int AreaSqm { get; set; }
        public List<string> HighlightedFeatures { get; set; } = new List<string>();
        public List<Amenity> Amenities { get; set; } = new List<Amenity>();
        public bool Featured { get; set; } = false;
        public List<string> Images { get; set; } = new List<string>();
        public string? CoverImage { get; set; }
        public List<string> Views360Url { get; set; } = new List<string>();
        public string City { get; set; } = null!;
        public string State { get; set; } = null!;
        public string Country { get; set; } = null!;
        public Location Location { get; set; } = null!;
        [BsonRepresentation(BsonType.ObjectId)]
        public string IdOwner { get; set; } = null!;
        public PropertyStatus Status { get; set; } = PropertyStatus.AVAILABLE;
        public PropertyTypes Type { get; set; } = PropertyTypes.OTHER;
    }

    public class Property : PropertyBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = null!;
        public string CodeInternal { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
