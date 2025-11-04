using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using RealEstate.Domain.Entities;

namespace RealEstate.Infrastructure.API.Conventions
{
    public static class MongoDbConventions
    {
        /// <summary>
        /// Registers the CamelCase convention to map C# properties
        /// (PascalCase) to MongoDB field names (camelCase).
        /// </summary>
        public static void RegisterCamelCaseConvention()
        {
            var pack = new ConventionPack
            {
                new CamelCaseElementNameConvention(),
                new IgnoreExtraElementsConvention(true), // Ignora campos extra en MongoDB
            };

            ConventionRegistry.Register(
                "CamelCaseConventionPack",
                pack,
                t => true);
        }

        /// <summary>
        /// Registers the class maps for entities with inheritance.
        /// </summary>
        public static void RegisterClassMaps()
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(PropertyBase)))
            {
                BsonClassMap.RegisterClassMap<PropertyBase>(cm =>
                {
                    cm.AutoMap();
                    cm.SetIsRootClass(true);
                });
            }

            if (!BsonClassMap.IsClassMapRegistered(typeof(Property)))
            {
                BsonClassMap.RegisterClassMap<Property>(cm =>
                {
                    cm.AutoMap();
                });
            }
        }
    }
}