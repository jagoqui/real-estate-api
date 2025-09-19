using System.Text;
using RealEstate.Application.Contracts;
using RealEstate.Domain.Entities;
using RealEstate.Infrastructure.API.Exceptions;

namespace RealEstate.Infrastructure.API.Services
{
    public class PropertyImageService : IPropertyImageService
    {
        private readonly IPropertyImageRepository _propertyImageRepository;
        private readonly IPropertyRepository _propertyRepository;

        public PropertyImageService(IPropertyImageRepository propertyImageRepository, IPropertyRepository propertyRepository)
        {
            _propertyImageRepository = propertyImageRepository ?? throw new ArgumentNullException(nameof(propertyImageRepository));
            _propertyRepository = propertyRepository ?? throw new ArgumentNullException(nameof(propertyRepository));
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
            try
            {
            return await EnsurePropertyImageExistsAsync(id);
            }
            catch (Exception ex)
            {
            throw new InternalServerErrorException($"Error retrieving property image with ID {id}.", ex);
            }
        }

        public async Task<IEnumerable<PropertyImage>> GetPropertyImagesByPropertyIdAsync(string propertyId)
        {
            try
            {
                return await _propertyImageRepository.GetPropertyImagesByPropertyIdAsync(propertyId);
            }
            catch (Exception ex)
            {
                throw new InternalServerErrorException("Error retrieving property images by property ID.", ex);
            }
        }

        public async Task<PropertyImage> AddPropertyImageAsync(PropertyImageWithoutId propertyImage)
        {
            if (propertyImage == null)
                throw new BadRequestException("Property image cannot be null.");

            await EnsurePropertyExistsAsync(propertyImage.IdProperty);

            ValidateImage(propertyImage.File);

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

            await EnsurePropertyExistsAsync(propertyImage.IdProperty);

            ValidateImage(propertyImage.File);

            try
            {
                await _propertyImageRepository.UpdatePropertyImageAsync(
                    existingPropertyImage.IdPropertyImage,
                    CreatePropertyImageWithId(propertyImage, existingPropertyImage.IdPropertyImage)
                );

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

        private async Task EnsurePropertyExistsAsync(string propertyId) 
        {
            if (string.IsNullOrWhiteSpace(propertyId))
                throw new BadRequestException("Property ID cannot be empty.");

            var property = await _propertyRepository.GetPropertyByIdAsync(propertyId);
            if (property == null)
                throw new BadRequestException($"No property found with ID {propertyId}.");
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

        /// <summary>
        /// Validates that the base64 string represents a valid image (JPG, PNG, GIF, WEBP)
        /// and that the file size does not exceed the allowed limit.
        /// </summary>
        private void ValidateImage(string base64File, int maxSizeInMB = 5)
        {
            if (string.IsNullOrWhiteSpace(base64File))
            throw new BadRequestException("File cannot be empty.");

            try
            {
            var base64Data = base64File.Contains(",")
                ? base64File.Split(',')[1]
                : base64File;

            var fileBytes = Convert.FromBase64String(base64Data);

            // Validate file size
            var sizeInMB = fileBytes.Length / (1024.0 * 1024.0);
            if (sizeInMB > maxSizeInMB)
                throw new BadRequestException($"File size exceeds the {maxSizeInMB} MB limit.");

            // Validate file format
            if (!IsImage(fileBytes))
                throw new BadRequestException("File is not a valid image (JPG, PNG, GIF, WEBP).");
            }
            catch (FormatException)
            {
            throw new BadRequestException("File is not a valid Base64 string.");
            }
        }

        private bool IsImage(byte[] fileBytes)
        {
            if (fileBytes.Length < 4) return false;

            // PNG
            if (fileBytes[0] == 0x89 && fileBytes[1] == 0x50 && fileBytes[2] == 0x4E && fileBytes[3] == 0x47)
                return true;

            // JPG
            if (fileBytes[0] == 0xFF && fileBytes[1] == 0xD8)
                return true;

            // GIF
            if (fileBytes[0] == 0x47 && fileBytes[1] == 0x49 && fileBytes[2] == 0x46)
                return true;

            // WEBP
            if (Encoding.ASCII.GetString(fileBytes, 0, 4) == "RIFF" &&
                Encoding.ASCII.GetString(fileBytes, 8, 4) == "WEBP")
                return true;

            return false;
        }
    }
}
