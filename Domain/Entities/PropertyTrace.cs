using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RealEstate.Domain.Entities
{
    public class PropertyTrace
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string IdPropertyTrace { get; set; } = null!;

        [BsonElement("DateSale")]
        public DateTime DateSale { get; set; }

        [BsonElement("Name")]
        public string Name { get; set; } = null!;

        [BsonElement("Value")]
        public decimal Value { get; set; }

        [BsonElement("Tax")]
        public decimal Tax { get; set; }

        // FK: Property
        [BsonRepresentation(BsonType.ObjectId)]
        public string IdProperty { get; set; } = null!;
    }
}
