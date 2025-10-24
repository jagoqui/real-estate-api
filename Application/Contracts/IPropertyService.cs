using RealEstate.Domain.Entities;
using RealEstate.Domain.Enums;
using RealEstate.Infrastructure.DTOs;

namespace RealEstate.Application.Contracts
{
    public interface IPropertyService
    {
        Task<IEnumerable<Property>> GetAllPropertiesAsync();
        Task<Property?> GetPropertyByIdAsync(string id);
        Task<IEnumerable<Property>> GetPropertiesByOwnerIdAsync(string ownerId);
        Task<Property> AddPropertyAsync(PropertyRequestDto propertyRequestDto);
        Task<Property> UpdatePropertyAsync(string id, PropertyRequestDto property);
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
