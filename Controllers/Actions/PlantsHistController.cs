using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using plant_api.Data;
using plant_api.Models;
using plant_api.Models.Actions;

namespace plant_api.Controllers.ApiActions
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class PlantsHistController : ControllerBase
    {
        private readonly PlantApiContext _context;

        public PlantsHistController(PlantApiContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlantsHist>>> GetApiActions()
        {
          if (_context.ApiActions == null)
          {
              return NotFound();
          }
            return await _context.ApiActions.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PlantsHist>> GetApiAction(long id)
        {
          if (_context.ApiActions == null)
          {
              return NotFound();
          }
            var apiAction = await _context.ApiActions.FindAsync(id);

            if (apiAction == null)
            {
                return NotFound();
            }

            return apiAction;
        }

        [HttpGet("GetByPlant/{id}")]
        public async Task<ActionResult<IEnumerable<PlantsHist>>> GetByPlantApiAction(long id)
        {
            if (_context.ApiActions == null)
            {
                return NotFound();
            }
            var apiAction = await _context.ApiActions.Where(hist => hist.PlantID == id)?.ToListAsync();

            if (apiAction == null)
            {
                return NotFound();
            }

            return apiAction;
        }

        [HttpGet("GetLastWatered/{id}")]
        public async Task<ActionResult<PlantsHist>> GetLastWateredApiAction(long id)
        {
            if (_context.ApiActions == null)
            {
                return NotFound();
            }
            var apiAction = await _context.ApiActions.Where(hist => hist.WateredPlant && hist.PlantID == id)?.OrderBy(hist => hist.Date)?.LastOrDefaultAsync();

            if (apiAction == null)
            {
                return NotFound();
            }

            return apiAction;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutApiAction(long id, PlantsHist apiAction)
        {
            if (id != apiAction.ID)
            {
                return BadRequest();
            }

            _context.Entry(apiAction).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ApiActionExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<PlantsHist>> PostApiAction(PlantHistCreate apiAction)
        {
          if (_context.ApiActions == null)
          {
              return Problem("Entity set 'PlantApiContext.ApiActions'  is null.");
          }
            var ID = await GenerateId();
            var create = new PlantsHist() { 
                ID = ID,
                PlantID = apiAction.plantID,
                Sunlight = apiAction.sunlight,
                Temperature = apiAction.temperature,
                AirHumidity = apiAction.airHumidity,
                SoilHumidity = apiAction.soilHumidity,
                WateredPlant = false,
                LampOn = false,
                FanOn = false,
                Date = DateTime.Now
            };
            _context.ApiActions.Add(create);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetApiAction", new { id = ID }, apiAction);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteApiAction(long id)
        {
            if (_context.ApiActions == null)
            {
                return NotFound();
            }
            var apiAction = await _context.ApiActions.FindAsync(id);
            if (apiAction == null)
            {
                return NotFound();
            }

            _context.ApiActions.Remove(apiAction);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ApiActionExists(long id)
        {
            return (_context.ApiActions?.Any(e => e.ID == id)).GetValueOrDefault();
        }

        private async Task<long> GenerateId()
        {
            try
            {
                if (!_context.ApiActions.Any())
                    return 1;
                return await _context.ApiActions?.MaxAsync(s => s.ID) + 1;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
