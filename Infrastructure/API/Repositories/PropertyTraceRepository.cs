using MongoDB.Driver;
using RealEstate.Application.Contracts;
using RealEstate.Domain.Entities;

namespace RealEstate.Infrastructure.API.Repositories
{
    public class PropertyTraceRepository : IPropertyTraceRepository
    {
        private readonly IMongoCollection<PropertyTrace> _propertyTraces;

        public PropertyTraceRepository(IMongoDatabase database)
        {
            _propertyTraces = database.GetCollection<PropertyTrace>("PropertyTraces");
        }

        public async Task<IEnumerable<PropertyTrace>> GetPropertyTracesAsync()
        {
            return await _propertyTraces.Find(_ => true).ToListAsync();
        }

        public async Task<PropertyTrace?> GetPropertyTraceByIdAsync(string id)
        {
            return await _propertyTraces.Find(pt => pt.IdPropertyTrace == id).FirstOrDefaultAsync();
        }

        public async Task<PropertyTrace> AddPropertyTraceAsync(PropertyTrace propertyTrace)
        {
            await _propertyTraces.InsertOneAsync(propertyTrace);
            return propertyTrace;
        }
    }
}