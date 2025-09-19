using RealEstate.Domain.Entities;
using RealEstate.Application.Contracts;
using RealEstate.Infrastructure.API.Exceptions;

namespace RealEstate.Infrastructure.API.Services
{
    public class PropertyTraceService : IPropertyTraceService
    {
        private readonly IPropertyTraceRepository _propertyTraceRepository;

        public PropertyTraceService(IPropertyTraceRepository propertyTraceRepository)
        {
            _propertyTraceRepository = propertyTraceRepository ?? throw new ArgumentNullException(nameof(propertyTraceRepository));
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

        public async Task<PropertyTrace> AddPropertyTraceAsync(PropertyTraceWithoutId propertyTrace)
        {
            if (propertyTrace == null)
                throw new BadRequestException("PropertyTrace cannot be null.");

            try
            {
                return await _propertyTraceRepository.AddPropertyTraceAsync(CreatePropertyTraceWithId(propertyTrace));
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

        private PropertyTrace CreatePropertyTraceWithId(PropertyTraceWithoutId propertyTrace)
        {
            return new PropertyTrace
            {
                DateSale = propertyTrace.DateSale,
                Name = propertyTrace.Name,
                Value = propertyTrace.Value,
                Tax = propertyTrace.Tax,
                IdProperty = propertyTrace.IdProperty
            };
        }
    }
}