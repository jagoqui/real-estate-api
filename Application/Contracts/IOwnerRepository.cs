using RealEstate.Domain.Entities;

namespace RealEstate.Application.Contracts
{
    public interface IOwnerRepository
    {
        Task<IEnumerable<Owner>> GetOwnersAsync();
        Task<Owner?> GetOwnerByIdAsync(string id);
        Task<Owner?> GetOwnerByUserIdAsync(string userId);
        Task<IEnumerable<Owner>> GetOwnersWithoutUserIdAsync();
        Task<Owner> AddOwnerAsync(Owner owner);
        Task<Owner?> UpdateOwnerAsync(Owner owner);
        Task DeleteOwnerAsync(string id);
    }
}
