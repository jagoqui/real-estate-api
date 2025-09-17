using RealEstate.Application.Contracts;
using RealEstate.Infrastructure.DTOs;

namespace RealEstate.Application.Services
{
    public class PropertyService : IPropertyService
    {
        private readonly IPropertyRepository _repository;

        public PropertyService(IPropertyRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<PropertyWithOwnerDto>> GetPropertiesAsync(
            string? name,
            string? address,
            decimal? minPrice,
            decimal? maxPrice)
        {
            return await _repository.GetPropertiesAsync(name, address, minPrice, maxPrice);
        }
    }
}
