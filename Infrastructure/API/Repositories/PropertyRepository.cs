using MongoDB.Driver;
using RealEstate.Domain.Entities;
using RealEstate.Application.Contracts;
using RealEstate.Infrastructure.DTOs;
using MongoDB.Bson;

namespace RealEstate.Infrastructure.API.Repositories
{
    public class PropertyRepository : IPropertyRepository
    {
        private readonly IMongoCollection<Property> _properties;
        private readonly IMongoCollection<Owner> _owners;
        private readonly IMongoCollection<PropertyImage> _images;

        public PropertyRepository(IMongoDatabase database)
        {
            _properties = database.GetCollection<Property>("Properties");
            _owners = database.GetCollection<Owner>("Owners");
            _images = database.GetCollection<PropertyImage>("PropertyImages");
        }

        public async Task<IEnumerable<PropertyWithOwnerDto>> GetPropertiesAsync(
            string? name,
            string? address,
            decimal? minPrice,
            decimal? maxPrice)
        {
            var filter = Builders<Property>.Filter.Empty;

            if (!string.IsNullOrEmpty(name))
                filter &= Builders<Property>.Filter.Regex(p => p.Name, new BsonRegularExpression(name, "i"));

            if (!string.IsNullOrEmpty(address))
                filter &= Builders<Property>.Filter.Regex(p => p.Address, new BsonRegularExpression(address, "i"));

            if (minPrice.HasValue)
                filter &= Builders<Property>.Filter.Gte(p => p.Price, minPrice.Value);

            if (maxPrice.HasValue)
                filter &= Builders<Property>.Filter.Lte(p => p.Price, maxPrice.Value);

            var propertyList = await _properties.Find(filter).ToListAsync();

            var result = from prop in propertyList
                         join owner in _owners.AsQueryable() on prop.IdOwner equals owner.IdOwner
                         join img in _images.AsQueryable() on prop.IdProperty equals img.IdProperty into propImgs
                         select new PropertyWithOwnerDto
                         {
                             IdOwner = owner.IdOwner,
                             Name = owner.Name,
                             Address = prop.Address,
                             Price = prop.Price,
                             Image = propImgs.FirstOrDefault(i => i.Enabled)?.File ?? string.Empty
                         };

            return result.ToList();
        }

        public async Task<Property?> GetByIdAsync(string id)
        {
            return await _properties.Find(p => p.IdProperty == id).FirstOrDefaultAsync();
        }

        public async Task<PropertyWithOwnerDto?> GetWithOwnerByIdAsync(string id)
        {
            var prop = await GetByIdAsync(id);
            if (prop == null) return null;

            var owner = await _owners.Find(o => o.IdOwner == prop.IdOwner).FirstOrDefaultAsync();
            var image = await _images.Find(i => i.IdProperty == prop.IdProperty && i.Enabled).FirstOrDefaultAsync();

            if (owner == null) return null;

            return new PropertyWithOwnerDto
            {
                IdOwner = owner.IdOwner,
                Name = owner.Name,
                Address = prop.Address,
                Price = prop.Price,
                Image = image?.File ?? string.Empty
            };
        }

        public async Task AddAsync(Property property)
        {
            await _properties.InsertOneAsync(property);
        }

        public async Task UpdateAsync(string id, Property property)
        {
            await _properties.ReplaceOneAsync(p => p.IdProperty == id, property);
        }

        public async Task DeleteAsync(string id)
        {
            await _properties.DeleteOneAsync(p => p.IdProperty == id);
        }
    }
}
