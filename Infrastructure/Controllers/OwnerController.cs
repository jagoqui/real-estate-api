using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using RealEstate.Domain.Entities;

namespace RealEstate.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OwnerController : ControllerBase
    {
        private readonly IMongoCollection<Owner> _owners;

        public OwnerController(IMongoDatabase database)
        {
            _owners = database.GetCollection<Owner>("Owners");
        }
        
        [HttpGet]
        public async Task<IActionResult> GetOwners()
        {
            var owners = await _owners.Find(_ => true).ToListAsync();
            return Ok(owners);
        }
    }
}
