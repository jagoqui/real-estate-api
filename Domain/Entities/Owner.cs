using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RealEstate.Domain.Entities
{
    public class OwnerWithoutId
    {
        [BsonElement("Name")]
        public string Name { get; set; } = null!;

        [BsonElement("Address")]
        public string Address { get; set; } = null!;

        [BsonElement("Photo")]
        public string Photo { get; set; } = null!;

        [BsonElement("Birthday")]
        public DateTime Birthday { get; set; }
    }

    public class Owner : OwnerWithoutId
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string IdOwner { get; set; } = null!;
    }
}
