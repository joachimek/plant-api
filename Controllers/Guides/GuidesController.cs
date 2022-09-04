using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using plant_api.Data;
using plant_api.Helpers;
using plant_api.Models.Guide;
using System.Security.Claims;

namespace plant_api.Controllers.Guides
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class GuidesController : ControllerBase
    {
        private readonly PlantApiContext _context;

        public GuidesController(PlantApiContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Models.Guides>>> GetGuides()
        {
            if (_context.Guides == null)
            {
                return NotFound();
            }
            
            var guides = await _context.Guides.ToListAsync();

            if (guides.Any())
            {
                return Ok(guides);
            }

            return NotFound();
        }

        [HttpGet("GetBySpeciesId/{speciesId}")]
        public async Task<ActionResult<IEnumerable<Models.Guides>>> GetGuidesBySpecies(long speciesId)
        {
            if (_context.Guides == null)
            {
                return NotFound();
            }

            var guides = await _context.Guides.Where(g => g.SpeciesID == speciesId).ToListAsync();

            if (guides.Any())
            {
                return Ok(guides);
            }

            return NotFound();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Models.Guides>> GetGuide(long id)
        {
            if (_context.Guides == null)
            {
                return NotFound();
            }

            var guide = await _context.Guides.FindAsync(id);

            if (guide == null)
            {
                return NotFound();
            }

            return Ok(guide);
        }

        [HttpPost]
        public async Task<ActionResult<Models.Guides>> InsertGuide(InsertGuideRequest request)
        {
            var userId = Identity.GetUserId(identity: HttpContext?.User?.Identity as ClaimsIdentity ?? new ClaimsIdentity());

            if (_context.Guides == null)
            {
                return Problem("Entity set 'PlantApiContext.Guides'  is null.");
            }

            var guide = new Models.Guides()
            {
                SpeciesID = request.SpeciesID,
                UserID = userId,
                Info = request.Info,
                MaxHumidity = request.MaxHumidity ?? 1.0,
                MinHumidity = request.MinHumidity ?? 0.0,
            };
            _context.Guides.Add(guide);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGuide", new { id = guide.ID }, guide);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGuide(long id, UpdateGuideRequest guide)
        {
            var userId = Identity.GetUserId(identity: HttpContext?.User?.Identity as ClaimsIdentity ?? new ClaimsIdentity());

            if (_context.Guides == null)
            {
                return NotFound();
            }

            var guideDb = await _context.Guides.FirstOrDefaultAsync(d => d.ID == id && d.UserID == userId);

            if (guideDb == null)
            {
                return NotFound();
            }

            guideDb.SpeciesID = guide.SpeciesID ?? guideDb.SpeciesID;
            guideDb.UserID = guide.UserID ?? guideDb.UserID;
            guideDb.Info = guide.Info ?? guideDb.Info;
            guideDb.MaxHumidity = guide.MaxHumidity ?? guideDb.MaxHumidity;
            guideDb.MinHumidity = guide.MinHumidity ?? guideDb.MinHumidity;

            _context.Entry(guideDb).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(guideDb);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GuideExists(id))
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
        public async Task<IActionResult> DeleteGuide(long id)
        {
            var userId = Identity.GetUserId(identity: HttpContext?.User?.Identity as ClaimsIdentity ?? new ClaimsIdentity());

            if (_context.Guides == null)
            {
                return NotFound();
            }

            var guide = await _context.Guides.FirstOrDefaultAsync(d => d.ID == id && d.UserID == userId);

            if (guide == null)
            {
                return NotFound();
            }

            _context.Guides.Remove(guide);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("ForceDelete/{id}")]
        public async Task<IActionResult> ForceDeleteGuide(long id)
        {
            if (_context.Guides == null)
            {
                return NotFound();
            }

            var guide = await _context.Guides.FirstOrDefaultAsync(d => d.ID == id);

            if (guide == null)
            {
                return NotFound();
            }

            _context.Guides.Remove(guide);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool GuideExists(long id)
        {
            return (_context.Guides?.Any(e => e.ID == id)).GetValueOrDefault();
        }
    }
}
