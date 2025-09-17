using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RealEstate.Domain.Entities
{
    public class PropertyImage
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string IdPropertyImage { get; set; } = null!;

        // FK: Property
        [BsonRepresentation(BsonType.ObjectId)]
        public string IdProperty { get; set; } = null!;

        [BsonElement("File")]
        public string File { get; set; } = null!;

        [BsonElement("Enabled")]
        public bool Enabled { get; set; }
    }
}
