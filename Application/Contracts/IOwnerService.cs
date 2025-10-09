using RealEstate.Domain.Entities;

namespace RealEstate.Application.Contracts
{
    public interface IOwnerService
    {
        Task<Owner> CreateOwnerAsync(OwnerWithoutOwnerId owner);
        Task<IEnumerable<Owner>> GetAllOwnersAsync();
        Task<Owner?> GetOwnerByIdAsync(string id);
        Task<Owner?> GetOwnerByUserIdAsync(string userId);
        Task<int> GetPropertiesCountByOwnerIdAsync(string ownerId);
        Task<Owner> UpdateOwnerAsync(string id, Owner owner);
        Task DeleteOwnerAsync(string id);
    }
}
