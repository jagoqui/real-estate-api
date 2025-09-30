using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using RealEstate.Domain.Enums;

namespace RealEstate.Domain.Entities
{
    public class UserWithoutId
    {
        [BsonElement("googleId")]
        public string GoogleId { get; set; } = null!;

        [BsonElement("email")]
        public string Email { get; set; } = null!;

        [BsonElement("name")]
        public string? Name { get; set; }

        [BsonElement("googleId")]
        public string? GoogleId { get; set; }

        [BsonElement("passwordHash")]
        public string? PasswordHash { get; set; }

        [BsonElement("role")]
        public UserRole Role { get; set; } = UserRole.OWNER;

        [BsonElement("refreshToken")]
        public string? RefreshToken { get; set; }

        [BsonElement("RefreshTokenExpiryTime")]
        public DateTime? RefreshTokenExpiryTime { get; set; }
    }

    public class User : UserWithoutId
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = null!;
    }
}
