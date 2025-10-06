using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstate.Application.Contracts;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Enums;

namespace RealEstate.Infrastructure.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OwnersController : ControllerBase
    {
        private readonly IOwnerService _ownerService;

        public OwnersController(IOwnerService ownerService)
        {
            _ownerService = ownerService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Owner>), StatusCodes.Status200OK)]
        [Authorize(Roles = nameof(UserRole.ADMIN))]
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
            return owner == null ? NotFound() : Ok(owner);
        }

        [HttpGet("user/{userId}")]
        [ProducesResponseType(typeof(Owner), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetOwnerByUserId(string userId)
        {
            var owner = await _ownerService.GetOwnerByUserIdAsync(userId);
            return owner == null ? NotFound() : Ok(owner);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Owner), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = nameof(UserRole.ADMIN))]
        public async Task<IActionResult> CreateOwner([FromBody] OwnerWithoutIds owner)
        {
            var createdOwner = await _ownerService.AddOwnerAsync(owner);
            return CreatedAtAction(nameof(GetOwnerById), new { id = createdOwner.IdOwner }, createdOwner);
        }

        [HttpGet("{id}/properties-count")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPropertiesCountByOwnerId(string id)
        {
            var count = await _ownerService.GetPropertiesCountByOwnerIdAsync(id);
            return Ok(count);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Owner), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateOwner(string id, [FromBody] Owner owner)
        {
            var updatedOwner = await _ownerService.UpdateOwnerAsync(id, owner);
            return updatedOwner == null ? NotFound() : Ok(updatedOwner);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Roles = nameof(UserRole.ADMIN))]
        public async Task<IActionResult> DeleteOwner(string id)
        {
            await _ownerService.DeleteOwnerAsync(id);
            return NoContent();
        }
    }
}
