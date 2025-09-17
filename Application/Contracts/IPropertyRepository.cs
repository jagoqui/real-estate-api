using RealEstate.Domain.Entities;
using RealEstate.Infrastructure.DTOs;

namespace RealEstate.Application.Contracts
{
    public interface IPropertyRepository
    {
        // Búsqueda con filtros (lista de DTOs combinados)
        Task<IEnumerable<PropertyWithOwnerDto>> GetPropertiesAsync(
            string? name,
            string? address,
            decimal? minPrice,
            decimal? maxPrice);

        // Obtener una propiedad por id (entidad)
        Task<Property?> GetByIdAsync(string id);

        // Obtener una propiedad + owner + imagen (DTO)
        Task<PropertyWithOwnerDto?> GetWithOwnerByIdAsync(string id);

        // CRUD básicos
        Task AddAsync(Property property);
        Task UpdateAsync(string id, Property property);
        Task DeleteAsync(string id);
    }
}
