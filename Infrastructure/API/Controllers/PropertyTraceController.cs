using Microsoft.AspNetCore.Mvc;
using RealEstate.Application.Contracts;
using RealEstate.Domain.Entities;

namespace RealEstate.Infrastructure.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PropertyTraceController : ControllerBase
    {
        private readonly IPropertyTraceService _propertyTraceService;

        public PropertyTraceController(IPropertyTraceService propertyTraceService)
        {
            _propertyTraceService = propertyTraceService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PropertyTrace>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllPropertyTraces()
        {
            var traces = await _propertyTraceService.GetAllPropertyTracesAsync();
            return Ok(traces);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PropertyTrace), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPropertyTraceById(string id)
        {
            var trace = await _propertyTraceService.GetPropertyTraceByIdAsync(id);
            if (trace == null)
                return NotFound();
            return Ok(trace);
        }

        [HttpPost]
        [ProducesResponseType(typeof(PropertyTrace), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreatePropertyTrace([FromBody] IPropertyTraceTax propertyTrace)
        {
            var createdTrace = await _propertyTraceService.AddPropertyTraceAsync(propertyTrace);
            return CreatedAtAction(nameof(GetPropertyTraceById), new { id = createdTrace.IdPropertyTrace }, createdTrace);
        }
    }
}