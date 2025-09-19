using RealEstate.Domain.Entities;

namespace RealEstate.Application.Contracts
{
    public interface IPropertyImageService
    {
        Task<IEnumerable<PropertyImage>> GetAllPropertyImagesAsync();
        Task<PropertyImage?> GetPropertyImageByIdAsync(string id);
        Task<PropertyImage> AddPropertyImageAsync(PropertyImageWithoutId propertyImage);
        Task<PropertyImage> UpdatePropertyImageAsync(string id, PropertyImageWithoutId propertyImage);
        Task DeletePropertyImageAsync(string id);
    }
}