﻿
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
        public async Task<ActionResult<Models.Plants>> InsertPlant(InsertPlantRequest request)
        {
            var userId = Identity.GetUserId(identity: HttpContext?.User?.Identity as ClaimsIdentity ?? new ClaimsIdentity());

            if (_context.Plants == null || _context.Devices == null || _context.Species == null)
            {
                return Problem("Entity set 'PlantApiContext.Plants', 'PlantApiContext.Species' or 'PlantApiContext.Devices'  is null.");
            }

            var device = await _context.Devices.FirstOrDefaultAsync(x => x.ID == request.DeviceID);
            var species = await _context.Species.FirstOrDefaultAsync(x => x.ID == request.SpeciesID);

            var plant = new Models.Plants()
            {
                SpeciesID = request.SpeciesID,
                Species = species ?? null,
                DeviceID = request.DeviceID,
                Device = device ?? null,
                GuideID = request.GuideID,
                Name = request.Name,
            };
            _context.Plants.Add(plant);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPlant", new { id = plant.ID }, plant);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePlant(long id, UpdatePlantRequest plant)
        {
            var userId = Identity.GetUserId(identity: HttpContext?.User?.Identity as ClaimsIdentity ?? new ClaimsIdentity());

            if (_context.Plants == null || _context.Devices == null || _context.Species == null)
            {
                return NotFound();
            }

            var plantDb = await _context.Plants.FirstOrDefaultAsync(p =>
                p.ID == id &&
                p.Device != null &&
                p.Device.UserID == userId
            );

            if (plantDb == null)
            {
                return NotFound();
            }


            if(plant.DeviceID != null)
            {
                var device = await _context.Devices.FirstOrDefaultAsync(x => x.ID == plant.DeviceID);

                if(device != null)
                {
                    plantDb.DeviceID = plant.DeviceID ?? plantDb.DeviceID;
                    plantDb.Device = device ?? plantDb.Device;
                    device.PlantID = id;
                    device.Plant = plantDb;

                    _context.Entry(device).State = EntityState.Modified;
                }
            }

            if(plant.SpeciesID != null)
            {
                var species = await _context.Species.FirstOrDefaultAsync(x => x.ID == plant.SpeciesID);

                plantDb.SpeciesID = plant.SpeciesID ?? plantDb.SpeciesID;
                plantDb.Species = species ?? plantDb.Species;
            }

            plantDb.Name = plant.Name ?? plantDb.Name;

            _context.Entry(plantDb).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(plantDb);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlantExists(id))
                {
                    return NotFound();
                }
                else
                {
                    return NoContent();
                }
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlant(long id)
        {
            var userId = Identity.GetUserId(identity: HttpContext?.User?.Identity as ClaimsIdentity ?? new ClaimsIdentity());

            if (_context.Plants == null)
            {
                return NotFound();
            }

            var plant = await _context.Plants.FirstOrDefaultAsync(d => d.ID == id && d.Device != null && d.Device.UserID == userId);

            if (plant == null)
            {
                return NotFound();
            }

            _context.Plants.Remove(plant);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("ForceDelete/{id}")]
        public async Task<IActionResult> ForceDeletePlant(long id)
        {
            if (_context.Plants == null)
            {
                return NotFound();
            }

            var plant = await _context.Plants.FirstOrDefaultAsync(d => d.ID == id);

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
    }
}
