using RealEstate.Domain.Entities;

namespace RealEstate.Application.Contracts
{
    public interface IPropertyRepository
    {
        Task<IEnumerable<Property>> GetPropertiesAsync();
        Task<Property?> GetPropertyByIdAsync(string id);
        Task<IEnumerable<Property>> GetPropertiesByOwnerIdAsync(string ownerId);
        Task<Property> AddPropertyAsync(Property property);
        Task<Property?> UpdatePropertyAsync(string id, Property property);
        Task DeletePropertyAsync(string id);
        Task<IEnumerable<Property>> GetPropertiesByFilterAsync(
            string? name = null,
            string? address = null,
            decimal? minPrice = null,
            decimal? maxPrice = null
        );
    }
}
