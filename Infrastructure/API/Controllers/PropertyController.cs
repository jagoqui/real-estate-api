using Microsoft.AspNetCore.Mvc;
using RealEstate.Application.Contracts;
using RealEstate.Domain.Entities;

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
        [ProducesResponseType(typeof(IEnumerable<Property>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllProperties()
        {
            var properties = await _propertyService.GetAllPropertiesAsync();
            return Ok(properties);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Property), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPropertyById(string id)
        {
            var property = await _propertyService.GetPropertyByIdAsync(id);
            if (property == null) return NotFound();
            return Ok(property);
        }

        [HttpGet("owner/{ownerId}")]
        [ProducesResponseType(typeof(Property), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPropertyByOwnerId(string ownerId)
        {
            var property = await _propertyService.GetPropertyByOwnerIdAsync(ownerId);
            if (property == null) return NotFound();
            return Ok(property);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Property), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateProperty([FromBody] PropertyWithoutId property)
        {
            var createdProperty = await _propertyService.AddPropertyAsync(property);
            return CreatedAtAction(nameof(GetPropertyById), new { id = createdProperty.IdProperty }, createdProperty);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Property), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateProperty(string id, [FromBody] PropertyWithoutId property)
        {
            var updatedProperty = await _propertyService.UpdatePropertyAsync(id, property);
            if (updatedProperty == null) return NotFound();
            return Ok(updatedProperty);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteProperty(string id)
        {
            await _propertyService.DeletePropertyAsync(id);
            return NoContent();
        }

        [HttpGet("filter")]
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
