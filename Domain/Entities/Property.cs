using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

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

        // FK: Owner
        [BsonRepresentation(BsonType.ObjectId)]
        public string IdOwner { get; set; } = null!;
    }
    public class Property: PropertyWithoutId
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string IdProperty { get; set; } = null!;
    }
}
