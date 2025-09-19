using MongoDB.Driver;
using RealEstate.Domain.Entities;
using RealEstate.Application.Contracts;

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
            return await _properties.Find(_ => true).ToListAsync();
        }

        public async Task<Property?> GetPropertyByIdAsync(string id)
        {
            return await _properties.Find(p => p.IdProperty == id).FirstOrDefaultAsync();
        }

        public async Task<Property?> GetPropertyByOwnerIdAsync(string ownerId)
        {
            return await _properties.Find(p => p.IdOwner == ownerId).FirstOrDefaultAsync();
        }

        public async Task<Property> AddPropertyAsync(Property property)
        {
            await _properties.InsertOneAsync(property);
            return property;
        }

        public async Task<Property?> UpdatePropertyAsync(string id, Property property)
        {
            await _properties.ReplaceOneAsync(p => p.IdProperty == id, property);
            return property;
        }

        public async Task DeletePropertyAsync(string id)
        {
            await _properties.DeleteOneAsync(p => p.IdProperty == id);
        }
    }
}
