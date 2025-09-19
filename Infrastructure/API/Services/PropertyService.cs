using RealEstate.Application.Contracts;
using RealEstate.Domain.Entities;
using RealEstate.Infrastructure.API.Exceptions;

namespace RealEstate.Infrastructure.API.Services
{
    public class PropertyService : IPropertyService
    {
        private readonly IPropertyRepository _propertyRepository;
        private readonly IPropertyImageRepository _propertyImageRepository;
        private readonly IOwnerRepository _ownerRepository;

        public PropertyService(IPropertyRepository propertyRepository, IPropertyImageRepository propertyImageRepository, IOwnerRepository ownerRepository)
        {
            _propertyRepository = propertyRepository ?? throw new ArgumentNullException(nameof(propertyRepository));
            _propertyImageRepository = propertyImageRepository ?? throw new ArgumentNullException(nameof(propertyImageRepository));
            _ownerRepository = ownerRepository ?? throw new ArgumentNullException(nameof(ownerRepository));
        }

        public async Task<IEnumerable<Property>> GetAllPropertiesAsync()
        {
            try
            {
                return await _propertyRepository.GetPropertiesAsync();
            }
            catch (Exception ex)
            {
                throw new InternalServerErrorException("Error retrieving properties.", ex);
            }
        }

        public async Task<Property?> GetPropertyByIdAsync(string id)
        {
            return await EnsurePropertyExistsAsync(id);
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

        public async Task<Property> AddPropertyAsync(PropertyWithoutId property)
        {
            if (property == null)
                throw new BadRequestException("Property cannot be null.");

            await EnsureOwnerExistsAsync(property.IdOwner);

            try
            {
                return await _propertyRepository.AddPropertyAsync(CreatePropertyWithId(property));
            }
            catch (Exception ex)
            {
                throw new InternalServerErrorException("Error adding property.", ex);
            }
        }

        public async Task<Property> UpdatePropertyAsync(string id, PropertyWithoutId property)
        {
            if (property == null)
                throw new BadRequestException("Property cannot be null.");

            Property existingProperty = await EnsurePropertyExistsAsync(id);

            await EnsureOwnerExistsAsync(property.IdOwner);

            try
            {
                await _propertyRepository.UpdatePropertyAsync(existingProperty.IdProperty, CreatePropertyWithId(property, existingProperty.IdProperty));

                return await _propertyRepository.GetPropertyByIdAsync(id)
                       ?? throw new InternalServerErrorException("Failed to retrieve the updated property.");
            }
            catch (Exception ex)
            {
                throw new InternalServerErrorException($"Error updating property with ID {id}.", ex);
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

        private async Task<Property> EnsurePropertyExistsAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new BadRequestException("Property ID cannot be empty.");

            var property = await _propertyRepository.GetPropertyByIdAsync(id);
            if (property == null)
                throw new NotFoundException($"No property found with ID {id}.");

            return property;
        }

        public async Task<IEnumerable<Property>> GetPropertiesByFilterAsync(
            string? name = null,
            string? address = null,
            decimal? minPrice = null,
            decimal? maxPrice = null
        )
        {
            try
            {
                return await _propertyRepository.GetPropertiesByFilterAsync(name, address, minPrice, maxPrice);
            }
            catch (Exception ex)
            {
                throw new InternalServerErrorException("Error retrieving properties with filters.", ex);
            }
        }

        private Property CreatePropertyWithId(PropertyWithoutId property, string? id = null)
        {
            return id != null ? new Property
            {
                IdProperty = id,
                Name = property.Name,
                Address = property.Address,
                Price = property.Price,
                CodeInternal = property.CodeInternal,
                Year = property.Year,
                IdOwner = property.IdOwner
            } : new Property
            {
                Name = property.Name,
                Address = property.Address,
                Price = property.Price,
                CodeInternal = property.CodeInternal,
                Year = property.Year,
                IdOwner = property.IdOwner
            };
        }

        private async Task EnsureOwnerExistsAsync(string ownerId)
        {
            if (string.IsNullOrWhiteSpace(ownerId))
                throw new BadRequestException("Owner ID cannot be empty.");

            var owner = await _ownerRepository.GetOwnerByIdAsync(ownerId);
            if (owner == null)
                throw new BadRequestException($"No owner found with ID {ownerId}.");
        }
    }
}
