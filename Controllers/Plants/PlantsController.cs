
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using plant_api.Data;
using plant_api.Helpers;
using plant_api.Models.Plant;

namespace plant_api.Controllers.Plants
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class PlantsController : ControllerBase
    {
        private readonly PlantApiContext _context;

        public PlantsController(PlantApiContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Models.Plants>> GetPlant(long id)
        {
            var userId = Identity.GetUserId(identity: HttpContext?.User?.Identity as ClaimsIdentity ?? new ClaimsIdentity());

            if (_context.Plants == null)
            {
                 return NotFound();
            }
            var plant = await _context.Plants.FirstOrDefaultAsync(p => p.ID == id && p.Device != null && p.Device.UserID == userId);

            if (plant == null)
            {
                return NotFound();
            }

            return plant;
        }

        [HttpGet("GetByDeviceId/{deviceId}")]
        public async Task<ActionResult<Models.Plants>> GetPlantByDeviceId(long deviceId)
        {
            var userId = Identity.GetUserId(identity: HttpContext?.User?.Identity as ClaimsIdentity ?? new ClaimsIdentity());

            if (_context.Plants == null)
            {
                return NotFound();
            }

            var plant = await _context.Plants.Include(x => x.Device).FirstOrDefaultAsync(p => p.DeviceID == deviceId && p.Device != null && p.Device.UserID == userId);

            if (plant == null)
            {
                return NotFound();
            }

            return plant;
        }

        [HttpPost]
        public async Task<ActionResult<Models.Plants>> InsertPlant(Models.Plants plant)
        {
            var userId = Identity.GetUserId(identity: HttpContext?.User?.Identity as ClaimsIdentity ?? new ClaimsIdentity());

            if (_context.Plants == null)
            {
                return Problem("Entity set 'PlantApiContext.Plants'  is null.");
            }

            plant.ID = await GenerateId();
            _context.Plants.Add(plant);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPlant", new { id = plant.ID }, plant);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePlant(long id, UpdatePlantRequest plant)
        {
            var userId = Identity.GetUserId(identity: HttpContext?.User?.Identity as ClaimsIdentity ?? new ClaimsIdentity());

            if (_context.Plants == null)
            {
                return NotFound();
            }

            var plantDb = await _context.Plants.FirstOrDefaultAsync(p => p.ID == id && p.Device.UserID == userId);

            if (plantDb == null)
            {
                return NotFound();
            }

            plantDb.Name = plant.Name;
            plantDb.DeviceID = plant.DeviceID;
            plantDb.SpeciesID = plant.SpeciesID;

            _context.Entry(plantDb).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlantExists(id))
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

        //TODO  add authority: admin or owner
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlant(long id)
        {
            if (_context.Plants == null)
            {
                return NotFound();
            }
            var plant = await _context.Plants.FindAsync(id);
            if (plant == null)
            {
                return NotFound();
            }

            _context.Plants.Remove(plant);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PlantExists(long id)
        {
            return (_context.Plants?.Any(e => e.ID == id)).GetValueOrDefault();
        }

        private async Task<long> GenerateId()
        {
            try
            {
                if (_context.Plants == null || !_context.Plants.Any())
                    return 1;
                return await _context.Plants.MaxAsync(s => s.ID) + 1;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
