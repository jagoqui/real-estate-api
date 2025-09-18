using Microsoft.AspNetCore.Mvc;
using RealEstate.Application.Contracts;
using RealEstate.Domain.Entities;

namespace RealEstate.Infrastructure.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OwnerController : ControllerBase
    {
        private readonly IOwnerService _ownerService;

        public OwnerController(IOwnerService ownerService)
        {
            _ownerService = ownerService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Owner>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllOwners()
        {
            var owners = await _ownerService.GetAllOwnersAsync();
            return Ok(owners);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Owner), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetOwnerById(string id)
        {
            var owner = await _ownerService.GetOwnerByIdAsync(id);
            if (owner == null) return NotFound();
            return Ok(owner); 
        }

        [HttpPost]
        [ProducesResponseType(typeof(Owner), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateOwner([FromBody] OwnerWithoutId owner)
        {
            var createdOwner = await _ownerService.AddOwnerAsync(owner);
            return CreatedAtAction(nameof(GetOwnerById), new { id = createdOwner.IdOwner }, createdOwner);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Owner), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateOwner(string id, [FromBody] OwnerWithoutId owner)
        {
            var updatedOwner = await _ownerService.UpdateOwnerAsync(id, owner);
            if (updatedOwner == null) return NotFound();
            return Ok(updatedOwner);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteOwner(string id)
        {
            await _ownerService.DeleteOwnerAsync(id);
            return NoContent();
        }
    }
}
