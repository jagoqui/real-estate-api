using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using RealEstate.Domain.Entities;
using RealEstate.Infrastructure.DTOs;

namespace RealEstate.Infrastructure.Persistence
{
    public class PropertyRepository
    {
        private readonly IMongoCollection<Property> _properties;
        private readonly IMongoCollection<Owner> _owners;

        public PropertyRepository(IMongoDatabase database)
        {
            _properties = database.GetCollection<Property>("Properties");
            _owners = database.GetCollection<Owner>("Owners");
        }

        // Obtener todas las propiedades
        public async Task<List<Property>> GetAllAsync()
        {
            return await _properties.Find(_ => true).ToListAsync();
        }

        // Insertar una nueva propiedad
        public async Task CreateAsync(Property property)
        {
            await _properties.InsertOneAsync(property);
        }

        // Filtrar propiedades por nombre, dirección y rango de precios
        public async Task<List<Property>> FilterAsync(string? name, string? address, decimal? minPrice, decimal? maxPrice)
        {
            var filterBuilder = Builders<Property>.Filter;
            var filters = new List<FilterDefinition<Property>>();

            if (!string.IsNullOrWhiteSpace(name))
                filters.Add(filterBuilder.Regex(p => p.Name, new BsonRegularExpression(name, "i")));

            if (!string.IsNullOrWhiteSpace(address))
                filters.Add(filterBuilder.Regex(p => p.Address, new BsonRegularExpression(address, "i")));

            if (minPrice.HasValue)
                filters.Add(filterBuilder.Gte(p => p.Price, minPrice.Value));

            if (maxPrice.HasValue)
                filters.Add(filterBuilder.Lte(p => p.Price, maxPrice.Value));

            var finalFilter = filters.Any() ? filterBuilder.And(filters) : FilterDefinition<Property>.Empty;
            return await _properties.Find(finalFilter).ToListAsync();
        }

        // Traer propiedades junto con la info del dueño
        public async Task<List<PropertyWithOwnerDto>> GetWithOwnersAsync()
        {
            var query = _properties.AsQueryable()
                          .Join(_owners.AsQueryable(),
                                p => p.IdOwner,
                                o => o.Id,
                                (p, o) => new PropertyWithOwnerDto
                                {
                                    Id = p.Id,
                                    Name = p.Name,
                                    Address = p.Address,
                                    Price = p.Price,
                                    ImageUrl = p.ImageUrl,
                                    OwnerId = o.Id,
                                    OwnerName = o.FullName,
                                    OwnerEmail = o.Email,
                                    OwnerPhone = o.Phone
                                });

            return await query.ToListAsync();
        }
    }
}
