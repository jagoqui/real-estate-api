using MongoDB.Driver;
using RealEstate.Application.Contracts;
using RealEstate.Domain.Entities;

namespace RealEstate.Infrastructure.API.Repositories
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

        public async Task<Owner?> GetOwnerByUserIdAsync(string userId)
        {
            return await _owners.Find(o => o.UserId == userId).FirstOrDefaultAsync();
        }

        public async Task<Owner> AddOwnerAsync(Owner owner)
        {
            await _owners.InsertOneAsync(owner);
            return owner;
        }

        public async Task<Owner?> UpdateOwnerAsync(string id, Owner owner)
        {
            await _owners.ReplaceOneAsync(o => o.IdOwner == id, owner);

            return owner;
        }

        public async Task DeleteOwnerAsync(string id)
        {
            await _owners.DeleteOneAsync(o => o.IdOwner == id);
        }
    }
}
