using RealEstate.Infrastructure.DTOs;

namespace RealEstate.Application.Contracts
{
    public interface IPropertyService
    {
        Task<IEnumerable<PropertyWithOwnerDto>> GetPropertiesAsync(
            string? name,
            string? address,
            decimal? minPrice,
            decimal? maxPrice);
    }
}
