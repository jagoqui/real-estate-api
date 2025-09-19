using RealEstate.Domain.Entities;

namespace RealEstate.Application.Contracts
{
    public interface IPropertyTraceRepository
    {
        Task<IEnumerable<PropertyTrace>> GetPropertyTracesAsync();
        Task<PropertyTrace?> GetPropertyTraceByIdAsync(string id);
        Task<PropertyTrace> AddPropertyTraceAsync(PropertyTrace propertyTrace);
    }
}