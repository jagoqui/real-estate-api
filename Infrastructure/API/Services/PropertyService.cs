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
                string? coverImageUrl = null;

                if (propertyRequestDTO.CoverImage != null)
                {
                    coverImageUrl = await _imageUploadService.UploadImageAsync(propertyRequestDTO.CoverImage, $"properties/{propertyRequestDTO.IdOwner}/cover");
                }

                return (await _propertyRepository.AddPropertyAsync(propertyRequestDTO.ToProperty(imageUrls, coverImageUrl))).ToPropertyResponseDto();
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
                var existingImagesByName = existingProperty.Images
                    .ToDictionary(GetFileNameFromUrl);

                var finalImageUrls = new List<string>();
                var filesToUpload = new List<IFormFile>();

                if (propertyRequestDTO.Images != null && propertyRequestDTO.Images.Any())
                {
                    foreach (var file in propertyRequestDTO.Images)
                    {
                        if (file == null || file.Length == 0)
                        {
                            continue;
                        }

                        if (existingImagesByName.TryGetValue(file.FileName, out var existingUrl))
                        {
                            finalImageUrls.Add(existingUrl);
                            existingImagesByName.Remove(file.FileName);
                        }
                        else
                        {
                            filesToUpload.Add(file);
                        }
                    }
                }

                if (filesToUpload.Any())
                {
                    var newImageUrls = await UploadNewImages(filesToUpload, propertyRequestDTO.IdOwner);

                    int newImageIndex = 0;
                    var finalUrls = new List<string>();

                    foreach (var file in propertyRequestDTO.Images!)
                    {
                        if (file == null || file.Length == 0)
                        {
                            continue;
                        }

                        var existingIndex = finalImageUrls.FindIndex(url => GetFileNameFromUrl(url) == file.FileName);
                        if (existingIndex >= 0)
                        {
                            finalUrls.Add(finalImageUrls[existingIndex]);
                        }
                        else if (newImageIndex < newImageUrls.Count)
                        {
                            finalUrls.Add(newImageUrls[newImageIndex]);
                            newImageIndex++;
                        }
                    }

                    finalImageUrls = finalUrls;
                }

                var imagesToDelete = existingImagesByName.Values.ToList();

                string? coverImageUrl = null;
                if (propertyRequestDTO.CoverImage != null)
                {
                    coverImageUrl = await _imageUploadService.UploadImageAsync(propertyRequestDTO.CoverImage, $"properties/{propertyRequestDTO.IdOwner}/cover", null, existingProperty.CoverImage);
                }

                await _propertyRepository.UpdatePropertyAsync(existingProperty.Id, propertyRequestDTO.ToProperty(finalImageUrls, coverImageUrl, existingProperty.Id));

                var propertyUpdated = await _propertyRepository.GetPropertyByIdAsync(id);

                if (propertyUpdated == null)
                    throw new InternalServerErrorException("Failed to retrieve the updated property.");

                if (imagesToDelete.Any())
                {
                    await _imageUploadService.DeleteImagesAsync(imagesToDelete);
                }

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
            Property existingProperty = await EnsurePropertyExistsAsync(id);

            await _imageUploadService.DeleteImagesAsync(existingProperty.Images);

            // TODO: Por el momento no esta quedando relacionado con el propertyImage
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
            PropertyFiltersRequestDto filters)
        {
            try
            {
                return (await _propertyRepository.GetPropertiesByFilterAsync(filters.ToPropertyFiltersDto())).Select(p => p.ToPropertyResponseDto());
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

        private async Task<List<string>> UploadNewImages(List<IFormFile> files, string ownerId)
        {
            List<string> uploadedImageUrls = new List<string>();

            if (files == null || !files.Any())
            {
                return uploadedImageUrls;
            }

            try
            {
                foreach (var file in files)
                {
                    if (file == null || file.Length == 0)
                    {
                        continue;
                    }

                    var imageUrl = await _imageUploadService.UploadImageAsync(file, $"properties/{ownerId}");
                    uploadedImageUrls.Add(imageUrl);
                }
            }
            catch (Exception ex)
            {
                throw new InternalServerErrorException($"Error uploading new images: {ex.Message}", ex);
            }

            return uploadedImageUrls;
        }

        private string GetFileNameFromUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return string.Empty;
            }

            try
            {
                var uri = new Uri(url);
                var segments = uri.AbsolutePath.Split('/');
                return segments.LastOrDefault() ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
