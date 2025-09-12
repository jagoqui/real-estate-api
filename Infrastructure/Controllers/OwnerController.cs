using Microsoft.AspNetCore.Mvc;
using RealEstate.Domain.Entities;
using RealEstate.Infrastructure.Persistence;

namespace RealEstate.Infrastructure.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OwnerController : ControllerBase
    {
        private readonly OwnerRepository _repository;

        public OwnerController(OwnerRepository repository)
        {
            _repository = repository;
        }

        // GET api/owner
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Owner>>> GetAll()
        {
            var owners = await _repository.GetAllAsync();
            return Ok(owners);
        }

        // POST api/owner
        [HttpPost]
        public async Task<ActionResult<Owner>> Create([FromBody] Owner owner)
        {
            await _repository.CreateAsync(owner);
            return CreatedAtAction(nameof(GetAll), new { id = owner.Id }, owner);
        }
    }
}
