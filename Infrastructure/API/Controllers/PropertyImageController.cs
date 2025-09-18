using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using RealEstate.Domain.Entities;

namespace RealEstate.Infrastructure.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PropertyImageController : ControllerBase
    {
        private readonly IMongoCollection<PropertyImage> _images;

        public PropertyImageController(IMongoDatabase database)
        {
            _images = database.GetCollection<PropertyImage>("PropertyImages");
        }

        /// <summary>
        /// Lista imágenes de una propiedad.
        /// </summary>
        [HttpGet("{propertyId}")]
        public async Task<IActionResult> GetImages(string propertyId)
        {
            var images = await _images.Find(img => img.IdProperty == propertyId).ToListAsync();
            return Ok(images);
        }
    }
}
