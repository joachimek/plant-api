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

            var plantsHists = await _context.ApiActions.Where(ph => ph.PlantID != -1).ToListAsync();

            if (plantsHists.Any())
            {
                return Ok(plantsHists);
            }

            return NotFound();
        }

        [HttpGet("GetMany/{ids}")]
        public async Task<ActionResult<IEnumerable<PlantsHist>>> GetManyPlantHists(string ids)
        {
            var idsParsed = ids.Split(',');
            long[] idsLong = idsParsed.Select(long.Parse).ToArray();

            if (_context.ApiActions == null)
            {
                return NotFound();
            }

            if (idsParsed != null && idsParsed.Length > 0)
            {
                var plantsHists = await _context.ApiActions.Where(d => idsLong.Contains(d.ID)).ToListAsync();
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

            var plantHist = await _context.ApiActions.FirstOrDefaultAsync(ph =>
                ph.ID == id &&
                ph.Plant != null &&
                ph.Plant.Device != null &&
                ph.Plant.Device.UserID == userId
                );

            if (plantHist == null)
            {
                return NotFound();
            }

            return Ok(plantHist);
        }

        [HttpGet("GetByPlantId/{id}")]
        public async Task<ActionResult<IEnumerable<PlantsHist>>> GetApiActionByPlantId(long plantId)
        {
            var userId = Identity.GetUserId(identity: HttpContext?.User?.Identity as ClaimsIdentity ?? new ClaimsIdentity());

            if (_context.ApiActions == null)
            {
                return NotFound();
            }

            var plantHist = await _context.ApiActions.Where(ph =>
                ph.PlantID == plantId &&
                ph.Plant != null &&
                ph.Plant.Device != null &&
                ph.Plant.Device.UserID == userId).ToListAsync();

            if (plantHist.Any())
            {
                return Ok(plantHist);
            }

            return NotFound();
        }

        [HttpGet("GetLastByPlantId/{id}")]
        public async Task<ActionResult<PlantsHist>> GetLastApiActionByPlantId(long plantId)
        {
            var userId = Identity.GetUserId(identity: HttpContext?.User?.Identity as ClaimsIdentity ?? new ClaimsIdentity());

            if (_context.ApiActions == null)
            {
                return NotFound();
            }

            var plantHist = await _context.ApiActions.Where(ph => ph.PlantID == plantId).OrderByDescending(p => p.Date).FirstOrDefaultAsync();

            if (plantHist != null)
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
                .ApiActions.Where(hist =>
                hist.WateredPlant &&
                hist.PlantID == id &&
                hist.Plant != null &&
                hist.Plant.Device != null &&
                hist.Plant.Device.UserID == userId
                ).OrderBy(hist => hist.Date)
                .LastOrDefaultAsync();

            if (plantHist == null)
            {
                return NotFound();
            }

            return Ok(plantHist);
        }

        [HttpPost]
        public async Task<ActionResult<PlantsHist>> InsertApiAction(InsertPlantHistRequest request)
        {
            var userId = Identity.GetUserId(identity: HttpContext?.User?.Identity as ClaimsIdentity ?? new ClaimsIdentity());

            if (_context.ApiActions == null )
            {
                return Problem("Entity set 'PlantApiContext.ApiActions'  is null.");
            }
            
            if (_context.Guides == null || _context.Plants == null || _context.Devices == null)
            {
                return BadRequest();
            }

            var device = await _context.Devices.FirstOrDefaultAsync(p => p.PlantID == request.PlantID);

            var plant = await _context.Plants.FirstOrDefaultAsync(p => p.ID == request.PlantID);

            if (plant == null || device == null || userId != device.UserID)
            {
                return BadRequest();
            }

            var guide = await _context.Guides.FirstOrDefaultAsync(p => p.ID == plant.GuideID);
            var minHumidity = guide?.MinHumidity ?? 0;
            var soilHumidity = request?.SoilHumidity ?? "1";

            var waterPlant = minHumidity > (Double.Parse(soilHumidity));

            var create = new PlantsHist() { 
                PlantID = request?.PlantID ?? -1,
                Sunlight = request?.Sunlight ?? false,
                Temperature = request?.Temperature ?? "NaN",
                AirHumidity = request?.AirHumidity ?? "NaN",
                SoilHumidity = request?.SoilHumidity ?? "NaN",
                WateredPlant = waterPlant,
                LampOn = false,
                FanOn = false,
                Date = DateTime.Now
            };
            _context.ApiActions.Add(create);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetApiAction", new { id = create.ID }, create);
        }

        [Authorize(Roles = "admin")]
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
    }
}
