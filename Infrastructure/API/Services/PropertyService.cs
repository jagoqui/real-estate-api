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

        public async Task<PropertyResponseDto> AddPropertyAsync(PropertyRequestDto propertyRequestDTO)
        {
            if (propertyRequestDTO == null)
                throw new BadRequestException("Property cannot be null.");

            await EnsureOwnerExistsAsync(propertyRequestDTO.IdOwner);

            try
            {
                var imageUrls = await LoadPropertyImages(propertyRequestDTO);
                Console.WriteLine("Image URLs: " + string.Join(", ", imageUrls));
                return (await _propertyRepository.AddPropertyAsync(propertyRequestDTO.ToProperty(imageUrls))).ToPropertyResponseDto();
            }
            catch (Exception ex)
            {
                throw new InternalServerErrorException("Error adding property.", ex);
            }
        }

        public async Task<IEnumerable<PropertyResponseDto>> GetAllPropertiesAsync()
        {
            try
            {
                return (await _propertyRepository.GetPropertiesAsync()).Select(p => p.ToPropertyResponseDto());
            }
            catch (Exception ex)
            {
                throw new InternalServerErrorException($"Error retrieving properties: {ex.Message}", ex);
            }
        }

        public async Task<PropertyResponseDto?> GetPropertyByIdAsync(string id)
        {
            try
            {
                var property = await _propertyRepository.GetPropertyByIdAsync(id);
                return property?.ToPropertyResponseDto();
            }
            catch (Exception ex)
            {
                throw new InternalServerErrorException($"Error retrieving property with ID {id}.", ex);
            }
        }

        public async Task<IEnumerable<PropertyResponseDto>> GetPropertiesByOwnerIdAsync(string ownerId)
        {
            try
            {
                return (await _propertyRepository.GetPropertiesByOwnerIdAsync(ownerId)).Select(p => p.ToPropertyResponseDto());
            }
            catch (Exception ex)
            {
                throw new InternalServerErrorException("Error retrieving properties by owner ID.", ex);
            }
        }

        public async Task<PropertyResponseDto> UpdatePropertyAsync(string id, PropertyRequestDto propertyRequestDTO)
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

                return propertyUpdated.ToPropertyResponseDto();
            }
            catch (Exception ex)
            {
                throw new InternalServerErrorException($"Error updating property with ID {id}.", ex);
            }
        }

        public async Task<PropertyResponseDto?> UpdatePropertyStatusAsync(string id, string status)
        {
            await EnsurePropertyExistsAsync(id);

            try
            {
                var statusParsed = Enum.TryParse<PropertyStatus>(status, out var parsedStatus) ? parsedStatus : PropertyStatus.AVAILABLE;
                return (await _propertyRepository.UpdatePropertyStatusAsync(id, statusParsed))?.ToPropertyResponseDto();
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

        public async Task<IEnumerable<PropertyResponseDto>> GetPropertiesByFilterAsync(
            string? name = null,
            string? address = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            PropertyStatus? status = null,
            PropertyTypes? type = null)
        {
            try
            {
                return (await _propertyRepository.GetPropertiesByFilterAsync(name, address, minPrice, maxPrice, status, type)).Select(p => p.ToPropertyResponseDto());
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

            if (propertyRequestDTO.Images == null || !propertyRequestDTO.Images.Any())
            {
                return uploadedImageUrls;
            }

            try
            {
                for (int i = 0; i < propertyRequestDTO.Images.Count; i++)
                {
                    var image = propertyRequestDTO.Images[i];

                    if (image == null || image.Length == 0)
                    {
                        continue;
                    }

                    var imageUrl = await _imageUploadService.UploadImageAsync(image, $"properties/{propertyRequestDTO.IdOwner}");
                    uploadedImageUrls.Add(imageUrl);
                }
            }
            catch (Exception ex)
            {
                throw new InternalServerErrorException($"Error uploading property images: {ex.Message}", ex);
            }

            return uploadedImageUrls;
        }
    }
}
