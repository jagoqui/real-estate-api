using RealEstate.Domain.Entities;
using RealEstate.Application.Contracts;
using RealEstate.Infrastructure.API.Exceptions;

namespace RealEstate.Infrastructure.API.Services
{
    public class PropertyService : IPropertyService
    {
        private readonly IPropertyRepository _propertyRepository;

        public PropertyService(IPropertyRepository propertyRepository)
        {
            _propertyRepository = propertyRepository ?? throw new ArgumentNullException(nameof(propertyRepository));
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

        public async Task<Property?> GetPropertyByOwnerIdAsync(string ownerId)
        {
            if (string.IsNullOrWhiteSpace(ownerId))
                throw new BadRequestException("Owner ID cannot be empty.");

            try
            {
                var property = await _propertyRepository.GetPropertyByOwnerIdAsync(ownerId);
                if (property == null)
                    throw new NotFoundException($"No property found for owner ID {ownerId}.");

                return property;
            }
            catch (Exception ex)
            {
                throw new InternalServerErrorException($"Error retrieving property for owner ID {ownerId}.", ex);
            }
        }

        public async Task<Property> AddPropertyAsync(PropertyWithoutId property)
        {
            if (property == null)
                throw new BadRequestException("Property cannot be null.");

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
    }
}
