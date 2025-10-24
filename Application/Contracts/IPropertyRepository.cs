using RealEstate.Domain.Entities;
using RealEstate.Domain.Enums;

namespace RealEstate.Application.Contracts
{
    public interface IPropertyRepository
    {
        Task<IEnumerable<Property>> GetPropertiesAsync();
        Task<Property?> GetPropertyByIdAsync(string id);
        Task<IEnumerable<Property>> GetPropertiesByOwnerIdAsync(string ownerId);
        Task<Property> AddPropertyAsync(Property property);
        Task<Property?> UpdatePropertyAsync(string id, Property property);
        Task<Property?> UpdatePropertyStatusAsync(string id, PropertyStatus status);
        Task DeletePropertyAsync(string id);
        Task<IEnumerable<Property>> GetPropertiesByFilterAsync(
            string? name = null,
            string? address = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            PropertyStatus? status = null,
            PropertyTypes? type = null);
    }
}
