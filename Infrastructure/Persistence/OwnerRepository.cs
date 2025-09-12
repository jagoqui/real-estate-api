using MongoDB.Driver;
using RealEstate.Domain.Entities;

namespace RealEstate.Infrastructure.Persistence
{
    public class OwnerRepository
    {
        private readonly IMongoCollection<Owner> _owners;

        public OwnerRepository(IMongoDatabase database)
        {
            _owners = database.GetCollection<Owner>("Owners");
        }

        public async Task<List<Owner>> GetAllAsync()
        {
            return await _owners.Find(_ => true).ToListAsync();
        }

        public async Task CreateAsync(Owner owner)
        {
            await _owners.InsertOneAsync(owner);
        }
    }
}
