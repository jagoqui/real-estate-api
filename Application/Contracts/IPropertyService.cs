using RealEstate.Domain.Entities;

namespace RealEstate.Application.Contracts
{
    public interface IPropertyService
    {
        Task<IEnumerable<Property>> GetAllPropertiesAsync();
        Task<Property?> GetPropertyByIdAsync(string id);
        Task<IEnumerable<Property>> GetPropertiesByOwnerIdAsync(string ownerId);
        Task<Property> AddPropertyAsync(PropertyWithoutId property);
        Task<Property> UpdatePropertyAsync(string id, PropertyWithoutId property);
        Task DeletePropertyAsync(string id);
        Task<IEnumerable<Property>> GetPropertiesByFilterAsync(
            string? name = null,
            string? address = null,
            decimal? minPrice = null,
            decimal? maxPrice = null);
    }
}
