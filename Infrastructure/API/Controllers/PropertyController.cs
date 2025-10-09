using Microsoft.AspNetCore.Mvc;
using RealEstate.Application.Contracts;
using RealEstate.Domain.Entities;
using Swashbuckle.AspNetCore.Annotations;

namespace RealEstate.Infrastructure.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PropertyController : ControllerBase
    {
        private readonly IPropertyService _propertyService;

        public PropertyController(IPropertyService propertyService)
        {
            _propertyService = propertyService;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Retrieves all properties.")]
        [ProducesResponseType(typeof(IEnumerable<Property>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllProperties()
        {
            var properties = await _propertyService.GetAllPropertiesAsync();
            return Ok(properties);
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Retrieves a property by its ID.")]
        [ProducesResponseType(typeof(Property), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPropertyById(string id)
        {
            var property = await _propertyService.GetPropertyByIdAsync(id);
            if (property == null)
                return NotFound();
            return Ok(property);
        }

        [HttpGet("owner/{ownerId}")]
        [SwaggerOperation(Summary = "Retrieves all properties associated with a specific owner ID.")]
        [ProducesResponseType(typeof(IEnumerable<Property>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPropertiesByOwnerId(string ownerId)
        {
            var properties = await _propertyService.GetPropertiesByOwnerIdAsync(ownerId);
            return Ok(properties);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Creates a new property.")]
        [ProducesResponseType(typeof(Property), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateProperty([FromBody] PropertyWithoutId property)
        {
            var createdProperty = await _propertyService.AddPropertyAsync(property);
            return CreatedAtAction(nameof(GetPropertyById), new { id = createdProperty.IdProperty }, createdProperty);
        }

        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Updates an existing property by its ID.")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(Property), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateProperty(string id, [FromBody] PropertyWithoutId property)
        {
            var updatedProperty = await _propertyService.UpdatePropertyAsync(id, property);
            if (updatedProperty == null)
                return NotFound();
            return Ok(updatedProperty);
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Deletes a property by its ID.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteProperty(string id)
        {
            await _propertyService.DeletePropertyAsync(id);
            return NoContent();
        }

        [HttpGet("filter")]
        [SwaggerOperation(Summary = "Retrieves properties based on optional filters: name, address, minPrice, maxPrice.")]
        [ProducesResponseType(typeof(IEnumerable<Property>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPropertiesByFilter(
            [FromQuery] string? name = null,
            [FromQuery] string? address = null,
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null)
        {
            var properties = await _propertyService.GetPropertiesByFilterAsync(name, address, minPrice, maxPrice);
            return Ok(properties);
        }
    }
}
