using RealEstate.Domain.Entities;

namespace RealEstate.Application.Contracts
{
    public interface IOwnerService
    {
        Task<IEnumerable<Owner>> GetAllOwnersAsync();
        Task<Owner?> GetOwnerByIdAsync(string id);
        Task<Owner?> GetOwnerByUserIdAsync(string userId);
        Task<Owner> AddOwnerAsync(OwnerWithoutIds owner);
        Task<Owner> UpdateOwnerAsync(string id, Owner owner);
        Task DeleteOwnerAsync(string id);
    }
}
