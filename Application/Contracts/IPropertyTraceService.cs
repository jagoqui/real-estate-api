using RealEstate.Domain.Entities;

namespace RealEstate.Application.Contracts
{
    public interface IPropertyTraceService
    {
        Task<IEnumerable<PropertyTrace>> GetAllPropertyTracesAsync();
        Task<PropertyTrace?> GetPropertyTraceByIdAsync(string id);
        Task<PropertyTrace> AddPropertyTraceAsync(PropertyTraceWithoutId propertyTrace);
    }
}