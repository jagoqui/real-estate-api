using RealEstate.Domain.Entities;

namespace RealEstate.Application.Contracts
{
    public interface IPropertyRepository
    {
        Task<IEnumerable<Property>> GetPropertiesAsync();
        Task<Property?> GetPropertyByIdAsync(string id);
        Task<Property> AddPropertyAsync(Property property);
        Task<Property?> UpdatePropertyAsync(string id, Property property);
        Task DeletePropertyAsync(string id);
    }
}
