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
        public async Task<IActionResult> GetAllOwners()
        {
            var owners = await _ownerService.GetAllOwnersAsync();
            return Ok(owners);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOwnerById(string id)
        {
            var owner = await _ownerService.GetOwnerByIdAsync(id);
            return Ok(owner); 
        }

        [HttpPost]
        public async Task<IActionResult> CreateOwner([FromBody] OwnerWithoutId owner)
        {

            var createdOwner = await _ownerService.AddOwnerAsync(owner);
            return CreatedAtAction(nameof(GetOwnerById), new { id = createdOwner.IdOwner }, createdOwner);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOwner(string id, [FromBody] OwnerWithoutId owner)
        {
            var updatedOwner = await _ownerService.UpdateOwnerAsync(id, owner);
            return Ok(updatedOwner);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOwner(string id)
        {
            await _ownerService.DeleteOwnerAsync(id);
            return NoContent();
        }
    }
}
