using MongoDB.Driver;
using RealEstate.Application.Contracts;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Enums;
using RealEstate.Infrastructure.DTOs;

namespace RealEstate.Infrastructure.API.Repositories
{
    public class PropertyRepository : IPropertyRepository
    {
        private readonly IMongoCollection<Property> _properties;

        public PropertyRepository(IMongoDatabase database)
        {
            _properties = database.GetCollection<Property>("Properties");
        }

        public async Task<IEnumerable<Property>> GetPropertiesAsync()
        {
            return await _properties.Find(static _ => true).ToListAsync();
        }

        public async Task<Property?> GetPropertyByIdAsync(string id)
        {
            return await _properties.Find(p => p.Id == id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Property>> GetPropertiesByOwnerIdAsync(string ownerId)
        {
            return await _properties.Find(p => p.IdOwner == ownerId).ToListAsync();
        }

        public async Task<Property> AddPropertyAsync(Property property)
        {
            await _properties.InsertOneAsync(property);
            return property;
        }

        public async Task<Property?> UpdatePropertyAsync(string id, Property property)
        {
            await _properties.ReplaceOneAsync(p => p.Id == id, property);
            return property;
        }

        public async Task<Property?> UpdatePropertyStatusAsync(string id, PropertyStatus status)
        {
            var update = Builders<Property>.Update.Set(p => p.Status, status);

            await _properties.UpdateOneAsync(p => p.Id == id, update);
            return await GetPropertyByIdAsync(id);
        }

        public async Task DeletePropertyAsync(string id)
        {
            await _properties.DeleteOneAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Property>> GetPropertiesByFilterAsync(PropertyFiltersDto filters)
        {
            var filterBuilder = Builders<Property>.Filter;
            var filterList = new List<FilterDefinition<Property>>();

            // Handle Name, Address, and Location as OR condition
            var textFilters = new List<FilterDefinition<Property>>();

            if (!string.IsNullOrEmpty(filters.Name))
            {
                textFilters.Add(filterBuilder.Or(
                    filterBuilder.Regex(p => p.Name, new MongoDB.Bson.BsonRegularExpression(filters.Name, "i")),
                    filterBuilder.Regex(p => p.Address, new MongoDB.Bson.BsonRegularExpression(filters.Name, "i")),
                    filterBuilder.Regex(p => p.City, new MongoDB.Bson.BsonRegularExpression(filters.Name, "i")),
                    filterBuilder.Regex(p => p.State, new MongoDB.Bson.BsonRegularExpression(filters.Name, "i")),
                    filterBuilder.Regex(p => p.Location.DisplayName, new MongoDB.Bson.BsonRegularExpression(filters.Name, "i"))));
            }

            if (!string.IsNullOrEmpty(filters.Address))
            {
                textFilters.Add(filterBuilder.Regex(p => p.Address, new MongoDB.Bson.BsonRegularExpression(filters.Address, "i")));
            }

            if (!string.IsNullOrEmpty(filters.Location))
            {
                textFilters.Add(filterBuilder.Regex(p => p.Location.DisplayName, new MongoDB.Bson.BsonRegularExpression(filters.Location, "i")));
            }

            if (textFilters.Any())
            {
                filterList.Add(filterBuilder.Or(textFilters));
            }

            if (filters.MinPrice.HasValue)
            {
                filterList.Add(filterBuilder.Gte(p => p.Price, filters.MinPrice.Value));
            }

            if (filters.MaxPrice.HasValue)
            {
                filterList.Add(filterBuilder.Lte(p => p.Price, filters.MaxPrice.Value));
            }

            if (filters.MinBedrooms.HasValue)
            {
                filterList.Add(filterBuilder.Gte(p => p.Bedrooms, filters.MinBedrooms.Value));
            }

            if (filters.MaxBedrooms.HasValue)
            {
                filterList.Add(filterBuilder.Lte(p => p.Bedrooms, filters.MaxBedrooms.Value));
            }

            if (filters.MinBathrooms.HasValue)
            {
                filterList.Add(filterBuilder.Gte(p => p.Bathrooms, filters.MinBathrooms.Value));
            }

            if (filters.MaxBathrooms.HasValue)
            {
                filterList.Add(filterBuilder.Lte(p => p.Bathrooms, filters.MaxBathrooms.Value));
            }

            if (filters.MinArea.HasValue)
            {
                filterList.Add(filterBuilder.Gte(p => p.AreaSqm, filters.MinArea.Value));
            }

            if (filters.MaxArea.HasValue)
            {
                filterList.Add(filterBuilder.Lte(p => p.AreaSqm, filters.MaxArea.Value));
            }

            if (filters.MinYear.HasValue)
            {
                filterList.Add(filterBuilder.Gte(p => p.Year, filters.MinYear.Value));
            }

            if (filters.MaxYear.HasValue)
            {
                filterList.Add(filterBuilder.Lte(p => p.Year, filters.MaxYear.Value));
            }

            if (filters.PropertyStatus.HasValue)
            {
                filterList.Add(filterBuilder.Eq(p => p.Status, filters.PropertyStatus.Value));
            }

            if (filters.PropertyType.HasValue)
            {
                filterList.Add(filterBuilder.Eq(p => p.Type, filters.PropertyType.Value));
            }

            FilterDefinition<Property> finalFilter;

            if (filterList.Any())
            {
                finalFilter = filterBuilder.And(filterList);
            }
            else
            {
                finalFilter = filterBuilder.Empty;
            }

            return await _properties.Find(finalFilter).ToListAsync();
        }
    }
}
