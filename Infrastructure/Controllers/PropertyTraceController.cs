using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using RealEstate.Domain.Entities;

namespace RealEstate.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PropertyTraceController : ControllerBase
    {
        private readonly IMongoCollection<PropertyTrace> _traces;

        public PropertyTraceController(IMongoDatabase database)
        {
            _traces = database.GetCollection<PropertyTrace>("PropertyTraces");
        }

        /// <summary>
        /// Lista el historial de ventas de una propiedad.
        /// </summary>
        [HttpGet("{propertyId}")]
        public async Task<IActionResult> GetTraces(string propertyId)
        {
            var traces = await _traces.Find(t => t.IdProperty == propertyId).ToListAsync();
            return Ok(traces);
        }
    }
}
