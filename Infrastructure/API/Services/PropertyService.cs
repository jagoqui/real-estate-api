using System.Threading.Tasks;
using RealEstate.Application.Adapters;
using RealEstate.Application.Contracts;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Enums;
using RealEstate.Infrastructure.API.Exceptions;
using RealEstate.Infrastructure.DTOs;

namespace RealEstate.Infrastructure.API.Services
{
    public class PropertyService : IPropertyService
    {
        private readonly IPropertyRepository _propertyRepository;
        private readonly IPropertyImageRepository _propertyImageRepository;
        private readonly IOwnerRepository _ownerRepository;

        private readonly IImageUploadService _imageUploadService;

        public PropertyService(IPropertyRepository propertyRepository, IPropertyImageRepository propertyImageRepository, IOwnerRepository ownerRepository, IImageUploadService imageUploadService)
        {
            _propertyRepository = propertyRepository ?? throw new ArgumentNullException(nameof(propertyRepository));
            _propertyImageRepository = propertyImageRepository ?? throw new ArgumentNullException(nameof(propertyImageRepository));
            _ownerRepository = ownerRepository ?? throw new ArgumentNullException(nameof(ownerRepository));
            _imageUploadService = imageUploadService ?? throw new ArgumentNullException(nameof(imageUploadService));
        }

        public async Task<Property> AddPropertyAsync(PropertyRequestDto propertyRequestDTO)
        {
            if (propertyRequestDTO == null)
                throw new BadRequestException("Property cannot be null.");

            await EnsureOwnerExistsAsync(propertyRequestDTO.IdOwner);

            try
            {
                var imageUrls = await LoadPropertyImages(propertyRequestDTO);
                return await _propertyRepository.AddPropertyAsync(propertyRequestDTO.ToProperty(imageUrls));
            }
            catch (Exception ex)
            {
                throw new InternalServerErrorException("Error adding property.", ex);
            }
        }

        public async Task<IEnumerable<Property>> GetAllPropertiesAsync()
        {
            try
            {
                return await _propertyRepository.GetPropertiesAsync();
            }
            catch (Exception ex)
            {
                throw new InternalServerErrorException($"Error retrieving properties: {ex.Message}", ex);
            }
        }

        public async Task<Property?> GetPropertyByIdAsync(string id)
        {
            try
            {
                return await _propertyRepository.GetPropertyByIdAsync(id);
            }
            catch (Exception ex)
            {
                throw new InternalServerErrorException($"Error retrieving property with ID {id}.", ex);
            }
        }

        public async Task<IEnumerable<Property>> GetPropertiesByOwnerIdAsync(string ownerId)
        {
            try
            {
                return await _propertyRepository.GetPropertiesByOwnerIdAsync(ownerId);
            }
            catch (Exception ex)
            {
                throw new InternalServerErrorException("Error retrieving properties by owner ID.", ex);
            }
        }

        public async Task<Property> UpdatePropertyAsync(string id, PropertyRequestDto propertyRequestDTO)
        {
            if (propertyRequestDTO == null)
                throw new BadRequestException("Property cannot be null.");

            Property existingProperty = await EnsurePropertyExistsAsync(id);

            await EnsureOwnerExistsAsync(propertyRequestDTO.IdOwner);

            try
            {
                var imageUrls = await LoadPropertyImages(propertyRequestDTO);

                await _propertyRepository.UpdatePropertyAsync(existingProperty.Id, propertyRequestDTO.ToProperty(imageUrls, existingProperty.Id));

                var propertyUpdated = await _propertyRepository.GetPropertyByIdAsync(id);

                if (propertyUpdated == null)
                    throw new InternalServerErrorException("Failed to retrieve the updated property.");

                await _imageUploadService.DeleteImagesAsync(existingProperty.Images);

                return propertyUpdated;
            }
            catch (Exception ex)
            {
                throw new InternalServerErrorException($"Error updating property with ID {id}.", ex);
            }
        }

        public async Task<Property?> UpdatePropertyStatusAsync(string id, PropertyStatus status)
        {
            await EnsurePropertyExistsAsync(id);

            try
            {
                return await _propertyRepository.UpdatePropertyStatusAsync(id, status);
            }
            catch (Exception ex)
            {
                throw new InternalServerErrorException($"Error updating status for property with ID {id}.", ex);
            }
        }

        public async Task DeletePropertyAsync(string id)
        {
            await EnsurePropertyExistsAsync(id);

            try
            {
                var images = await _propertyImageRepository.GetPropertyImagesByPropertyIdAsync(id);
                foreach (var image in images)
                {
                    await _propertyImageRepository.DeletePropertyImageAsync(image.IdPropertyImage);
                }

                await _propertyRepository.DeletePropertyAsync(id);
            }
            catch (Exception ex)
            {
                throw new InternalServerErrorException($"Error deleting property with ID {id}.", ex);
            }
        }

        public async Task<IEnumerable<Property>> GetPropertiesByFilterAsync(
            string? name = null,
            string? address = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            PropertyStatus? status = null,
            PropertyTypes? type = null)
        {
            try
            {
                return await _propertyRepository.GetPropertiesByFilterAsync(name, address, minPrice, maxPrice, status, type);
            }
            catch (Exception ex)
            {
                throw new InternalServerErrorException("Error retrieving properties with filters.", ex);
            }
        }

        private async Task<Property> EnsurePropertyExistsAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new BadRequestException("Property ID cannot be empty.");

            var property = await _propertyRepository.GetPropertyByIdAsync(id);
            if (property == null)
                throw new NotFoundException($"No property found with ID {id}.");

            return property;
        }

        private async Task EnsureOwnerExistsAsync(string ownerId)
        {
            if (string.IsNullOrWhiteSpace(ownerId))
                throw new BadRequestException("Owner ID cannot be empty.");

            var owner = await _ownerRepository.GetOwnerByIdAsync(ownerId);
            if (owner == null)
                throw new BadRequestException($"No owner found with ID {ownerId}.");
        }

        private async Task<List<string>> LoadPropertyImages(PropertyRequestDto propertyRequestDTO)
        {
            List<string> uploadedImageUrls = new List<string>();

            foreach (var image in propertyRequestDTO.Images)
            {
                var imageUrl = await _imageUploadService.UploadImageAsync(image, "properties");
                uploadedImageUrls.Add(imageUrl);
            }

            return uploadedImageUrls;
        }
    }
}
