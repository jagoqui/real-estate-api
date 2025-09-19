using MongoDB.Driver;
using RealEstate.Domain.Entities;
using RealEstate.Application.Contracts;

namespace RealEstate.Infrastructure.API.Repositories
{
    public class PropertyImageRepository : IPropertyImageRepository
    {
        private readonly IMongoCollection<PropertyImage> _propertyImages;

        public PropertyImageRepository(IMongoDatabase database)
        {
            _propertyImages = database.GetCollection<PropertyImage>("PropertyImages");
        }

        public async Task<IEnumerable<PropertyImage>> GetPropertyImagesAsync()
        {
            return await _propertyImages.Find(_ => true).ToListAsync();
        }

        public async Task<PropertyImage?> GetPropertyImageByIdAsync(string id)
        {
            return await _propertyImages.Find(pi => pi.IdPropertyImage == id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<PropertyImage>> GetPropertyImagesByPropertyIdAsync(string propertyId)
        {
            return await _propertyImages.Find(pi => pi.IdProperty == propertyId).ToListAsync();
        }

        public async Task<PropertyImage> AddPropertyImageAsync(PropertyImage propertyImage)
        {
            await _propertyImages.InsertOneAsync(propertyImage);
            return propertyImage;
        }

        public async Task<PropertyImage?> UpdatePropertyImageAsync(string id, PropertyImage propertyImage)
        {
            await _propertyImages.ReplaceOneAsync(pi => pi.IdPropertyImage == id, propertyImage);
            return propertyImage;
        }

        public async Task<PropertyImage?> UpdatePropertyImageFileAsync(string idPropertyImage, string base64File)
        {
            var update = Builders<PropertyImage>.Update.Set(pi => pi.File, base64File);
            var result = await _propertyImages.FindOneAndUpdateAsync(pi => pi.IdPropertyImage == idPropertyImage, update, new FindOneAndUpdateOptions<PropertyImage>
            {
                ReturnDocument = ReturnDocument.After
            });
            return result;
        }

        public async Task DeletePropertyImageAsync(string id)
        {
            await _propertyImages.DeleteOneAsync(pi => pi.IdPropertyImage == id);
        }
    }
}