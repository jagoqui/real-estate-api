using Microsoft.AspNetCore.Mvc;
using RealEstate.Application.Contracts;

namespace RealEstate.Infrastructure.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PropertyController : ControllerBase
    {
        private readonly IPropertyService _propertyService;

        public PropertyController(IPropertyService propertyService)
        {
            Console.WriteLine("PropertyController instantiated.");
            _propertyService = propertyService;
        }

        /// <summary>
        /// Obtiene la lista de propiedades con dueño e imagen principal.
        /// Permite filtrar por nombre, dirección y rango de precios.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetProperties(
            [FromQuery] string? name,
            [FromQuery] string? address,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice)
        {
            var properties = await _propertyService.GetPropertiesAsync(name, address, minPrice, maxPrice);

            if (!properties.Any())
                return NotFound("No se encontraron propiedades con los filtros aplicados.");

            return Ok(properties);
        }
    }
}
