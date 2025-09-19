using RealEstate.Domain.Entities;

namespace RealEstate.Application.Contracts
{
    public interface IPropertyImageService
    {
        Task<IEnumerable<PropertyImage>> GetAllPropertyImagesAsync();
        Task<PropertyImage?> GetPropertyImageByIdAsync(string id);
        Task<PropertyImage> AddPropertyImageAsync(PropertyImageWithoutId propertyImage);
        Task<PropertyImage> UpdatePropertyImageAsync(string id, PropertyImageWithoutId propertyImage);
        Task<PropertyImage?> UpdatePropertyImageFileAsync(string idPropertyImage, string base64File);
        Task DeletePropertyImageAsync(string id);
    }
}