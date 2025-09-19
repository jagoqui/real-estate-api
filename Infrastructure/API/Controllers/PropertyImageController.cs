using Microsoft.AspNetCore.Mvc;
using RealEstate.Application.Contracts;
using RealEstate.Domain.Entities;

namespace RealEstate.Infrastructure.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PropertyImageController : ControllerBase
    {
        private readonly IPropertyImageService _propertyImageService;

        public PropertyImageController(IPropertyImageService propertyImageService)
        {
            _propertyImageService = propertyImageService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PropertyImage>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllPropertyImages()
        {
            var images = await _propertyImageService.GetAllPropertyImagesAsync();
            return Ok(images);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PropertyImage), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPropertyImageById(string id)
        {
            var image = await _propertyImageService.GetPropertyImageByIdAsync(id);
            if (image == null) return NotFound();
            return Ok(image);
        }

        [HttpPost]
        [ProducesResponseType(typeof(PropertyImage), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreatePropertyImage([FromBody] PropertyImageWithoutId propertyImage)
        {
            var createdImage = await _propertyImageService.AddPropertyImageAsync(propertyImage);
            return CreatedAtAction(nameof(GetPropertyImageById), new { id = createdImage.IdPropertyImage }, createdImage);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(PropertyImage), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdatePropertyImage(string id, [FromBody] PropertyImageWithoutId propertyImage)
        {
            var updatedImage = await _propertyImageService.UpdatePropertyImageAsync(id, propertyImage);
            if (updatedImage == null) return NotFound();
            return Ok(updatedImage);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeletePropertyImage(string id)
        {
            await _propertyImageService.DeletePropertyImageAsync(id);
            return NoContent();
        }
    }
}