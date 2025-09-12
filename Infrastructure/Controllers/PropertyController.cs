using Microsoft.AspNetCore.Mvc;
using RealEstate.Domain.Entities;
using RealEstate.Infrastructure.DTOs;
using RealEstate.Infrastructure.Persistence;

namespace RealEstate.Infrastructure.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PropertyController : ControllerBase
    {
        private readonly PropertyRepository _repository;

        public PropertyController(PropertyRepository repository)
        {
            _repository = repository;
        }

        // GET api/property
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PropertyResponseDto>>> GetAll()
        {
            var properties = await _repository.GetAllAsync();

            var response = properties.Select(p => new PropertyResponseDto
            {
                Id = p.Id,
                IdOwner = p.IdOwner,
                Name = p.Name,
                Address = p.Address,
                Price = p.Price,
                ImageUrl = p.ImageUrl
            });

            return Ok(response);
        }

        // POST api/property
        [HttpPost]
        public async Task<ActionResult<PropertyResponseDto>> Create([FromBody] PropertyRequestDto dto)
        {
            if (dto == null)
                return BadRequest("Invalid property data");

            var property = new Property
            {
                IdOwner = dto.IdOwner,
                Name = dto.Name,
                Address = dto.Address,
                Price = dto.Price,
                ImageUrl = dto.ImageUrl
            };

            await _repository.CreateAsync(property);

            var response = new PropertyResponseDto
            {
                Id = property.Id,
                IdOwner = property.IdOwner,
                Name = property.Name,
                Address = property.Address,
                Price = property.Price,
                ImageUrl = property.ImageUrl
            };

            return CreatedAtAction(nameof(GetAll), new { id = response.Id }, response);
        }

        // GET api/property/filter
        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<PropertyResponseDto>>> Filter(
            [FromQuery] string? name,
            [FromQuery] string? address,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice)
        {
            var properties = await _repository.FilterAsync(name, address, minPrice, maxPrice);

            var response = properties.Select(p => new PropertyResponseDto
            {
                Id = p.Id,
                IdOwner = p.IdOwner,
                Name = p.Name,
                Address = p.Address,
                Price = p.Price,
                ImageUrl = p.ImageUrl
            });

            return Ok(response);
        }

        // GET api/property/details
        [HttpGet("details")]
        public async Task<ActionResult<IEnumerable<PropertyWithOwnerDto>>> GetWithOwners()
        {
            var properties = await _repository.GetWithOwnersAsync();
            return Ok(properties);
        }
    }
}
