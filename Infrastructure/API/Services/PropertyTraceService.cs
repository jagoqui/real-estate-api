using RealEstate.Domain.Entities;
using RealEstate.Application.Contracts;
using RealEstate.Infrastructure.API.Exceptions;

namespace RealEstate.Infrastructure.API.Services
{
    public class PropertyTraceService : IPropertyTraceService
    {
        private readonly IPropertyTraceRepository _propertyTraceRepository;
        private readonly IPropertyRepository _propertyRepository;

        public PropertyTraceService(IPropertyTraceRepository propertyTraceRepository, IPropertyRepository propertyRepository)
        {
            _propertyTraceRepository = propertyTraceRepository ?? throw new ArgumentNullException(nameof(propertyTraceRepository));
            _propertyRepository = propertyRepository ?? throw new ArgumentNullException(nameof(propertyRepository));
        }

        public async Task<IEnumerable<PropertyTrace>> GetAllPropertyTracesAsync()
        {
            try
            {
                return await _propertyTraceRepository.GetPropertyTracesAsync();
            }
            catch (Exception ex)
            {
                throw new InternalServerErrorException("Error retrieving property traces.", ex);
            }
        }

        public async Task<PropertyTrace?> GetPropertyTraceByIdAsync(string id)
        {
            return await EnsurePropertyTraceExistsAsync(id);
        }

        public async Task<PropertyTrace> AddPropertyTraceAsync(IPropertyTraceTax propertyTrace)
        {
            if (propertyTrace == null)
                throw new BadRequestException("PropertyTrace cannot be null.");

            Property property = await EnsurePropertyExistsAsync(propertyTrace.IdProperty);

            try
            {
                return await _propertyTraceRepository.AddPropertyTraceAsync(CreatePropertyTraceWithId(property,propertyTrace));
            }
            catch (Exception ex)
            {
                throw new InternalServerErrorException("Error adding property trace.", ex);
            }
        }

        private async Task<PropertyTrace> EnsurePropertyTraceExistsAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new BadRequestException("PropertyTrace ID cannot be empty.");

            var propertyTrace = await _propertyTraceRepository.GetPropertyTraceByIdAsync(id);
            if (propertyTrace == null)
                throw new NotFoundException($"No property trace found with ID {id}.");

            return propertyTrace;
        }

        private PropertyTrace CreatePropertyTraceWithId(Property property, IPropertyTraceTax propertyTrace)
        {
            return new PropertyTrace
            {
                DateSale = DateTime.Now,
                Name = property.Name,
                Value = property.Price,
                Tax = propertyTrace.Tax,
                IdProperty = propertyTrace.IdProperty
            };
        }

        private async Task<Property> EnsurePropertyExistsAsync(string propertyId)
        {
            if (string.IsNullOrWhiteSpace(propertyId))
                throw new BadRequestException("Property ID cannot be empty.");

            var property = await _propertyRepository.GetPropertyByIdAsync(propertyId);
            if (property == null)
                throw new BadRequestException($"No property found with ID {propertyId}.");

            return property;
        }
    }
}