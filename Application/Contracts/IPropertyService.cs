using RealEstate.Domain.Entities;

namespace RealEstate.Application.Contracts
{
    public interface IPropertyService
    {
        Task<IEnumerable<Property>> GetAllPropertiesAsync();
        Task<Property?> GetPropertyByIdAsync(string id);
        Task<Property> AddPropertyAsync(PropertyWithoutId property);
        Task<Property> UpdatePropertyAsync(string id, PropertyWithoutId property);
        Task DeletePropertyAsync(string id);
    }
}
