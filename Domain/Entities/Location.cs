using MongoDB.Bson.Serialization.Attributes;

namespace RealEstate.Domain.Entities
{
    public class Location
    {
        [BsonElement("Lat")]
        public double Lat { get; set; }

        [BsonElement("Lon")]
        public double Lon { get; set; }
    }
}