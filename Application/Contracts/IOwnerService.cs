using RealEstate.Domain.Entities;

namespace RealEstate.Application.Contracts
{
    public interface IOwnerService
    {
        Task<IEnumerable<Owner>> GetAllOwnersAsync();
        Task<Owner?> GetOwnerByIdAsync(string id);
        Task<Owner> AddOwnerAsync(OwnerWithoutId owner);
        Task<Owner> UpdateOwnerAsync(string id, OwnerWithoutId owner);
        Task DeleteOwnerAsync(string id);
    }
}
