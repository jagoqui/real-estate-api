using RealEstate.Domain.Entities;
using RealEstate.Application.Contracts;
using RealEstate.Infrastructure.API.Exceptions;

namespace RealEstate.Infrastructure.API.Services
{
    public class PropertyImageService : IPropertyImageService
    {
        private readonly IPropertyImageRepository _propertyImageRepository;

        public PropertyImageService(IPropertyImageRepository propertyImageRepository)
        {
            _propertyImageRepository = propertyImageRepository ?? throw new ArgumentNullException(nameof(propertyImageRepository));
        }

        public async Task<IEnumerable<PropertyImage>> GetAllPropertyImagesAsync()
        {
            try
            {
                return await _propertyImageRepository.GetPropertyImagesAsync();
            }
            catch (Exception ex)
            {
                throw new InternalServerErrorException("Error retrieving property images.", ex);
            }
        }

        public async Task<PropertyImage?> GetPropertyImageByIdAsync(string id)
        {
            return await EnsurePropertyImageExistsAsync(id);
        }

        public async Task<PropertyImage> AddPropertyImageAsync(PropertyImageWithoutId propertyImage)
        {
            if (propertyImage == null)
                throw new BadRequestException("Property image cannot be null.");

            try
            {
                return await _propertyImageRepository.AddPropertyImageAsync(CreatePropertyImageWithId(propertyImage));
            }
            catch (Exception ex)
            {
                throw new InternalServerErrorException("Error adding property image.", ex);
            }
        }

        public async Task<PropertyImage> UpdatePropertyImageAsync(string id, PropertyImageWithoutId propertyImage)
        {
            if (propertyImage == null)
                throw new BadRequestException("Property image cannot be null.");

            PropertyImage existingPropertyImage = await EnsurePropertyImageExistsAsync(id);

            try
            {
                await _propertyImageRepository.UpdatePropertyImageAsync(existingPropertyImage.IdPropertyImage, CreatePropertyImageWithId(propertyImage, existingPropertyImage.IdPropertyImage));

                return await _propertyImageRepository.GetPropertyImageByIdAsync(id)
                       ?? throw new InternalServerErrorException("Failed to retrieve the updated property image.");
            }
            catch (Exception ex)
            {
                throw new InternalServerErrorException($"Error updating property image with ID {id}.", ex);
            }
        }

        public async Task<PropertyImage?> UpdatePropertyImageFileAsync(string idPropertyImage, string base64File)
        {
            await EnsurePropertyImageExistsAsync(idPropertyImage);

            ValidateImage(base64File);

            try
            {
                var updatedImage = await _propertyImageRepository.UpdatePropertyImageFileAsync(idPropertyImage, base64File);
                if (updatedImage == null)
                    throw new InternalServerErrorException("Failed to update the property image file.");

                return updatedImage;
            }
            catch (Exception ex)
            {
                throw new InternalServerErrorException($"Error updating property image file with ID {idPropertyImage}.", ex);
            }
        }
        

        public async Task DeletePropertyImageAsync(string id)
        {
            await EnsurePropertyImageExistsAsync(id);

            try
            {
                await _propertyImageRepository.DeletePropertyImageAsync(id);
            }
            catch (Exception ex)
            {
                throw new InternalServerErrorException($"Error deleting property image with ID {id}.", ex);
            }
        }

        private async Task<PropertyImage> EnsurePropertyImageExistsAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new BadRequestException("Property image ID cannot be empty.");

            var propertyImage = await _propertyImageRepository.GetPropertyImageByIdAsync(id);
            if (propertyImage == null)
                throw new NotFoundException($"No property image found with ID {id}.");

            return propertyImage;
        }

        private PropertyImage CreatePropertyImageWithId(PropertyImageWithoutId propertyImage, string? id = null)
        {
            return id != null ? new PropertyImage
            {
                IdPropertyImage = id,
                IdProperty = propertyImage.IdProperty,
                File = propertyImage.File,
                Enabled = propertyImage.Enabled
            } : new PropertyImage
            {
                IdProperty = propertyImage.IdProperty,
                File = propertyImage.File,
                Enabled = propertyImage.Enabled
            };
        }
    }
}