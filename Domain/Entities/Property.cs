using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using RealEstate.Domain.Enums;

namespace RealEstate.Domain.Entities
{
    public class PropertyWithoutId
    {
        [BsonElement("Name")]
        public string Name { get; set; } = null!;

        [BsonElement("Address")]
        public string Address { get; set; } = null!;

        [BsonElement("Price")]
        public decimal Price { get; set; }

        [BsonElement("CodeInternal")]
        public string CodeInternal { get; set; } = null!;

        [BsonElement("Year")]
        public int Year { get; set; }

        [BsonElement("Description")]
        public string Description { get; set; } = null!;

        [BsonElement("Bathrooms")]
        public int Bathrooms { get; set; }

        [BsonElement("Bedrooms")]
        public int Bedrooms { get; set; }

        [BsonElement("AreaSqm")]
        public int AreaSqm { get; set; }

        [BsonElement("Status")]
        public PropertyStatus Status { get; set; } = PropertyStatus.AVAILABLE;

        [BsonElement("Features")]
        public List<string> Features { get; set; } = new List<string>();

        [BsonElement("Featured")]
        public bool Featured { get; set; } = false;

        [BsonElement("VirtualTourUrl")]
        public string? VirtualTourUrl { get; set; }

        // FK: Owner
        [BsonRepresentation(BsonType.ObjectId)]
        public string IdOwner { get; set; } = null!;
    }

    public class Property : PropertyWithoutId
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string IdProperty { get; set; } = null!;
    }
}
