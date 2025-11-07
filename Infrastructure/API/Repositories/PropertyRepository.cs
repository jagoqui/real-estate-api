using MongoDB.Driver;
using RealEstate.Application.Contracts;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Enums;

namespace RealEstate.Infrastructure.API.Repositories
{
    public class PropertyRepository : IPropertyRepository
    {
        private readonly IMongoCollection<Property> _properties;

        public PropertyRepository(IMongoDatabase database)
        {
            _properties = database.GetCollection<Property>("Properties");
        }

        public async Task<IEnumerable<Property>> GetPropertiesAsync()
        {
            return await _properties.Find(static _ => true).ToListAsync();
        }

        public async Task<Property?> GetPropertyByIdAsync(string id)
        {
            return await _properties.Find(p => p.Id == id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Property>> GetPropertiesByOwnerIdAsync(string ownerId)
        {
            return await _properties.Find(p => p.IdOwner == ownerId).ToListAsync();
        }

        public async Task<Property> AddPropertyAsync(Property property)
        {
            await _properties.InsertOneAsync(property);
            return property;
        }

        public async Task<Property?> UpdatePropertyAsync(string id, Property property)
        {
            await _properties.ReplaceOneAsync(p => p.Id == id, property);
            return property;
        }

        public async Task<Property?> UpdatePropertyStatusAsync(string id, PropertyStatus status)
        {
            var update = Builders<Property>.Update.Set(p => p.Status, status);

            await _properties.UpdateOneAsync(p => p.Id == id, update);
            return await GetPropertyByIdAsync(id);
        }

        public async Task DeletePropertyAsync(string id)
        {
            await _properties.DeleteOneAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Property>> GetPropertiesByFilterAsync(
            string? name = null,
            string? address = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            PropertyStatus? status = null,
            PropertyTypes? type = null)
        {
            var filterBuilder = Builders<Property>.Filter;
            var filter = filterBuilder.Empty;

            if (!string.IsNullOrEmpty(name))
            {
                filter &= filterBuilder.Regex(static p => p.Name, new MongoDB.Bson.BsonRegularExpression(name, "i"));
            }

            if (!string.IsNullOrEmpty(address))
            {
                filter &= filterBuilder.Regex(static p => p.Address, new MongoDB.Bson.BsonRegularExpression(address, "i"));
            }

            if (minPrice.HasValue)
            {
                filter &= filterBuilder.Gte(static p => p.Price, minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                filter &= filterBuilder.Lte(static p => p.Price, maxPrice.Value);
            }

            if (status.HasValue)
            {
                filter &= filterBuilder.Eq(static p => p.Status, status.Value);
            }

            if (type.HasValue)
            {
                filter &= filterBuilder.Eq(static p => p.Type, type.Value);
            }

            return await _properties.Find(filter).ToListAsync();
        }
    }
}
