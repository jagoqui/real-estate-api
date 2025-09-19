using RealEstate.Domain.Entities;

namespace RealEstate.Application.Contracts
{
    public interface IOwnerRepository
    {
        Task<IEnumerable<Owner>> GetOwnersAsync();
        Task<Owner?> GetOwnerByIdAsync(string id);
        Task<Owner> AddOwnerAsync(Owner owner);
        Task<Owner?> UpdateOwnerAsync(string id, Owner owner);
        Task DeleteOwnerAsync(string id);
    }
}
