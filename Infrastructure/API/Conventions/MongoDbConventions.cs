using MongoDB.Bson.Serialization.Conventions;

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
            };

            ConventionRegistry.Register(
                "CamelCaseConventionPack",
                pack,
                t => true);
        }
    }
}