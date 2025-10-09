using Microsoft.AspNetCore.Mvc;
using RealEstate.Application.Contracts;
using RealEstate.Domain.Entities;
using Swashbuckle.AspNetCore.Annotations;

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
        [SwaggerOperation(Summary = "Retrieves all property images.")]
        [ProducesResponseType(typeof(IEnumerable<PropertyImage>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllPropertyImages()
        {
            var images = await _propertyImageService.GetAllPropertyImagesAsync();
            return Ok(images);
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Retrieves a property image by its ID.")]
        [ProducesResponseType(typeof(PropertyImage), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPropertyImageById(string id)
        {
            var image = await _propertyImageService.GetPropertyImageByIdAsync(id);
            if (image == null)
                return NotFound();
            return Ok(image);
        }

        [HttpGet("property/{propertyId}")]
        [SwaggerOperation(Summary = "Retrieves all images associated with a specific property ID.")]
        [ProducesResponseType(typeof(IEnumerable<PropertyImage>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPropertyImagesByPropertyId(string propertyId)
        {
            var images = await _propertyImageService.GetPropertyImagesByPropertyIdAsync(propertyId);
            if (images == null || !images.Any())
                return NotFound();
            return Ok(images);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Creates a new property image.")]
        [ProducesResponseType(typeof(PropertyImage), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreatePropertyImage([FromBody] PropertyImageWithoutId propertyImage)
        {
            var createdImage = await _propertyImageService.AddPropertyImageAsync(propertyImage);
            return CreatedAtAction(nameof(GetPropertyImageById), new { id = createdImage.IdPropertyImage }, createdImage);
        }

        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Updates an existing property image by its ID.")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(PropertyImage), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdatePropertyImage(string id, [FromBody] PropertyImageWithoutId propertyImage)
        {
            var updatedImage = await _propertyImageService.UpdatePropertyImageAsync(id, propertyImage);
            if (updatedImage == null)
                return NotFound();
            return Ok(updatedImage);
        }

        [HttpPatch("{idPropertyImage}/file")]
        [SwaggerOperation(Summary = "Updates the file of an existing property image by its ID.")]
        [ProducesResponseType(typeof(PropertyImage), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdatePropertyImageFile(string idPropertyImage, [FromBody] string base64File)
        {
            var updatedImage = await _propertyImageService.UpdatePropertyImageFileAsync(idPropertyImage, base64File);
            if (updatedImage == null)
                return NotFound();
            return Ok(updatedImage);
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Deletes a property image by its ID.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeletePropertyImage(string id)
        {
            await _propertyImageService.DeletePropertyImageAsync(id);
            return NoContent();
        }
    }
}
