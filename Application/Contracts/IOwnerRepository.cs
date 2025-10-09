using RealEstate.Domain.Entities;

namespace RealEstate.Application.Contracts
{
    public interface IOwnerRepository
    {
        Task<Owner> CreateOwnerAsync(Owner owner);
        Task<IEnumerable<Owner>> GetOwnersAsync();
        Task<Owner?> GetOwnerByIdAsync(string id);
        Task<Owner?> GetOwnerByUserIdAsync(string userId);
        Task<Owner?> UpdateOwnerAsync(Owner owner);
        Task DeleteOwnerAsync(string id);
    }
}
