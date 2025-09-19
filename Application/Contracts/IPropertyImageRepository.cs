using RealEstate.Domain.Entities;

namespace RealEstate.Application.Contracts
{
    public interface IPropertyImageRepository
    {
        Task<IEnumerable<PropertyImage>> GetPropertyImagesAsync();
        Task<PropertyImage?> GetPropertyImageByIdAsync(string id);
        Task<PropertyImage> AddPropertyImageAsync(PropertyImage propertyImage);
        Task<PropertyImage?> UpdatePropertyImageAsync(string id, PropertyImage propertyImage);
        Task<PropertyImage?> UpdatePropertyImageFileAsync(string idPropertyImage, string base64File);
        Task DeletePropertyImageAsync(string id);
    }
}