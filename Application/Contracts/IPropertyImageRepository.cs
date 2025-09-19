using RealEstate.Domain.Entities;

namespace RealEstate.Application.Contracts
{
    public interface IPropertyImageRepository
    {
        Task<IEnumerable<PropertyImage>> GetPropertyImagesAsync();
        Task<PropertyImage?> GetPropertyImageByIdAsync(string id);
        Task<PropertyImage> AddPropertyImageAsync(PropertyImage propertyImage);
        Task<PropertyImage?> UpdatePropertyImageAsync(string id, PropertyImage propertyImage);
        Task DeletePropertyImageAsync(string id);
    }
}