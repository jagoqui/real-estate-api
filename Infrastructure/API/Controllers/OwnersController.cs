using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstate.Application.Contracts;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Enums;
using Swashbuckle.AspNetCore.Annotations;

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

        [HttpPost]
        [SwaggerOperation(Summary = "Creates a new owner. Admin role required.")]
        [ProducesResponseType(typeof(OwnerWithoutOwnerId), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = nameof(UserRole.ADMIN))]
        public async Task<IActionResult> CreateOwner([FromBody] OwnerWithoutOwnerId owner)
        {
            var createdOwner = await _ownerService.CreateOwnerAsync(owner);
            return CreatedAtAction(nameof(GetOwnerById), new { id = createdOwner.IdOwner }, createdOwner);
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Retrieves all owners. Admin role required.")]
        [ProducesResponseType(typeof(IEnumerable<Owner>), StatusCodes.Status200OK)]
        [Authorize(Roles = nameof(UserRole.ADMIN))]
        public async Task<IActionResult> GetAllOwners()
        {
            var owners = await _ownerService.GetAllOwnersAsync();
            return Ok(owners);
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Retrieves an owner by their ID.")]
        [ProducesResponseType(typeof(Owner), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetOwnerById(string id)
        {
            var owner = await _ownerService.GetOwnerByIdAsync(id);
            return owner == null ? NotFound() : Ok(owner);
        }

        [HttpGet("user/{userId}")]
        [SwaggerOperation(Summary = "Retrieves an owner by their associated user ID.")]
        [ProducesResponseType(typeof(Owner), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetOwnerByUserId(string userId)
        {
            var owner = await _ownerService.GetOwnerByUserIdAsync(userId);
            return owner == null ? NotFound() : Ok(owner);
        }

        [HttpGet("{id}/properties-count")]
        [SwaggerOperation(Summary = "Gets the count of properties associated with a specific owner ID.")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPropertiesCountByOwnerId(string id)
        {
            var count = await _ownerService.GetPropertiesCountByOwnerIdAsync(id);
            return Ok(count);
        }

        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Updates an existing owner by their ID.")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(Owner), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateOwner(string id, [FromBody] Owner owner)
        {
            var updatedOwner = await _ownerService.UpdateOwnerAsync(id, owner);
            return updatedOwner == null ? NotFound() : Ok(updatedOwner);
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Deletes an owner by their ID. Admin role required.")]
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
