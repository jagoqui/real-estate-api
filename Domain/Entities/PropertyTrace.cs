using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RealEstate.Domain.Entities
{
    public class IPropertyTraceTax
    {
        [BsonElement("Tax")]
        public decimal Tax { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string IdProperty { get; set; } = null!;
    }

    public class PropertyTraceWithoutId : IPropertyTraceTax
    {
        [BsonElement("DateSale")]
        public DateTime DateSale { get; set; }

        [BsonElement("Name")]
        public string Name { get; set; } = null!;

        [BsonElement("Value")]
        public decimal Value { get; set; }
    }

    public class PropertyTrace : PropertyTraceWithoutId
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string IdPropertyTrace { get; set; } = null!;
    }
}
