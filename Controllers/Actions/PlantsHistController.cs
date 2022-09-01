﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using plant_api.Data;
using plant_api.Helpers;
using plant_api.Models;
using plant_api.Models.Actions;
using System.Security.Claims;

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
            var userId = Identity.GetUserId(identity: HttpContext?.User?.Identity as ClaimsIdentity ?? new ClaimsIdentity());

            if (_context.ApiActions == null)
            {
                return NotFound();
            }

            var plantsHists = await _context.ApiActions.Where(d => d.Plant.Device.UserID == userId).ToListAsync();

            if (plantsHists.Any())
            {
                return Ok(plantsHists);
            }

            return NotFound();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PlantsHist>> GetApiAction(long id)
        {
            var userId = Identity.GetUserId(identity: HttpContext?.User?.Identity as ClaimsIdentity ?? new ClaimsIdentity());

            if (_context.ApiActions == null)
            {
                return NotFound();
            }

            var plantHist = await _context.ApiActions.FirstOrDefaultAsync(ph => ph.ID == id && ph.Plant.Device.UserID == userId);

            if (plantHist == null)
            {
                return NotFound();
            }

            return Ok(plantHist);
        }

        [HttpGet("GetByPlant/{id}")]
        public async Task<ActionResult<IEnumerable<PlantsHist>>> GetApiActionByPlant(long plantId)
        {
            var userId = Identity.GetUserId(identity: HttpContext?.User?.Identity as ClaimsIdentity ?? new ClaimsIdentity());

            if (_context.ApiActions == null)
            {
                return NotFound();
            }

            var plantHist = await _context.ApiActions.Where(ph => ph.PlantID == plantId && ph.Plant.Device.UserID == userId).ToListAsync();

            if (plantHist.Any())
            {
                return Ok(plantHist);
            }

            return NotFound();
        }

        [HttpGet("GetLastWatered/{id}")]
        public async Task<ActionResult<PlantsHist>> GetLastWateredApiAction(long id)
        {
            var userId = Identity.GetUserId(identity: HttpContext?.User?.Identity as ClaimsIdentity ?? new ClaimsIdentity());

            if (_context.ApiActions == null)
            {
                return NotFound();
            }

            var plantHist = await _context
                .ApiActions.Where(hist => hist.WateredPlant && hist.PlantID == id && hist.Plant.Device.UserID == userId)
                .OrderBy(hist => hist.Date)
                .LastOrDefaultAsync();

            if (plantHist == null)
            {
                return NotFound();
            }

            return Ok(plantHist);
        }

        [HttpPost]
        public async Task<ActionResult<PlantsHist>> InsertApiAction(PlantHistCreate apiAction)
        {
            var userId = Identity.GetUserId(identity: HttpContext?.User?.Identity as ClaimsIdentity ?? new ClaimsIdentity());

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

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateApiAction(long id, PlantsHist apiAction)
        {
            var userId = Identity.GetUserId(identity: HttpContext?.User?.Identity as ClaimsIdentity ?? new ClaimsIdentity());

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
                if (_context.ApiActions == null || !_context.ApiActions.Any())
                    return 1;
                return await _context.ApiActions.MaxAsync(s => s.ID) + 1;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
