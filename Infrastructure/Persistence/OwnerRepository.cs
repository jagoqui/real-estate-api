using MongoDB.Driver;
using RealEstate.Domain.Entities;
using RealEstate.Application.Contracts;

namespace RealEstate.Infrastructure.Persistence
{
    public class OwnerRepository : IOwnerRepository
    {
        private readonly IMongoCollection<Owner> _owners;

        public OwnerRepository(IMongoDatabase database)
        {
            _owners = database.GetCollection<Owner>("Owners");
        }

        public async Task<IEnumerable<Owner>> GetOwnersAsync()
        {
            return await _owners.Find(_ => true).ToListAsync();
        }

        public async Task<Owner?> GetOwnerByIdAsync(string id)
        {
            return await _owners.Find(o => o.IdOwner == id).FirstOrDefaultAsync();
        }

        public async Task AddOwnerAsync(Owner owner)
        {
            await _owners.InsertOneAsync(owner);
        }

        public async Task UpdateOwnerAsync(string id, Owner owner)
        {
            await _owners.ReplaceOneAsync(o => o.IdOwner == id, owner);
        }

        public async Task DeleteOwnerAsync(string id)
        {
            await _owners.DeleteOneAsync(o => o.IdOwner == id);
        }
    }
}
