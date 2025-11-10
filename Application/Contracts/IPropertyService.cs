using RealEstate.Domain.Entities;
using RealEstate.Domain.Enums;
using RealEstate.Infrastructure.DTOs;

namespace RealEstate.Application.Contracts
{
    public interface IPropertyService
    {
        Task<IEnumerable<PropertyResponseDto>> GetAllPropertiesAsync();
        Task<PropertyResponseDto?> GetPropertyByIdAsync(string id);
        Task<IEnumerable<PropertyResponseDto>> GetPropertiesByOwnerIdAsync(string ownerId);
        Task<PropertyResponseDto> AddPropertyAsync(PropertyRequestDto propertyRequestDto);
        Task<PropertyResponseDto> UpdatePropertyAsync(string id, PropertyRequestDto property);
        Task<PropertyResponseDto?> UpdatePropertyStatusAsync(string id, string status);
        Task DeletePropertyAsync(string id);
        Task<IEnumerable<PropertyResponseDto>> GetPropertiesByFilterAsync(PropertyFiltersRequestDto filters);
    }
}
