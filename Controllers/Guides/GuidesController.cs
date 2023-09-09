using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using plant_api.Data;
using plant_api.Helpers;
using plant_api.Models;
using plant_api.Models.Guide;
using System.Security.Claims;
using System.Linq.Dynamic.Core;

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

        [HttpGet("GetList/{props}")]
        public async Task<ActionResult<IEnumerable<Models.Guides>>> GetGuidesList(string props)
        {
            if (props == null)
            {
                return BadRequest();
            }

            if (_context.Guides == null)
            {
                return NotFound();
            }

            string[] propArray = props.Split('+');
            string sortField = propArray[0];
            string sortOrder = propArray[1];
            string page = propArray[2];
            string perPage = propArray[3];

            if (sortField == null || sortOrder == null || page == null || perPage == null)
            {
                return BadRequest();
            }

            var pageInt = Int32.Parse(page);
            var perPageInt = Int32.Parse(perPage);

            if (sortOrder == "ASC")
            {
                var species = _context.Guides
                    .OrderBy("s=>s." + sortField)
                    .AsEnumerable()
                    .Where((s, i) => i >= (pageInt - 1) * perPageInt && i < pageInt * perPageInt)
                    .ToList();
                return Ok(species);
            }
            else
            {
                var species = _context.Guides
                    .OrderBy("s=>s." + sortField + " DESC")
                    .AsEnumerable()
                    .Where((s, i) => i >= (pageInt - 1) * perPageInt && i < pageInt * perPageInt)
                    .ToList();
                return Ok(species);
            }
        }

        [HttpGet("GetMany/{ids}")]
        public async Task<ActionResult<IEnumerable<Models.Guides>>> GetManyGuides(string ids)
        {
            var idsParsed = ids.Split(',');
            long[] idsLong = idsParsed.Select(long.Parse).ToArray();

            if (_context.Guides == null)
            {
                return NotFound();
            }

            if (idsParsed != null && idsParsed.Length > 0)
            {
                var guides = await _context.Guides.Where(d => idsLong.Contains(d.ID)).ToListAsync();
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

        [HttpGet("GetByPlantId/{id}")]
        public async Task<ActionResult<Models.Guides>> GetGuideByPlantId(long id)
        {
            if (_context.Plants == null || _context.Guides == null)
            {
                return NotFound();
            }
            
            var plant = await _context.Plants.FindAsync(id);

            if (plant == null || plant.GuideID == -1)
            {
                return NotFound();
            }

            var guide = await _context.Guides.FindAsync(plant.GuideID);

            if (guide != null)
            {
                return Ok(guide);
            }

            return NotFound();
        }
        [HttpPost("GetMany")]
        public async Task<ActionResult<IEnumerable<Models.Guides>>> GetMany(Models.Common.GetManyRequest request)
        {
            if (_context.Guides == null)
            {
                return NotFound();
            }
            var guide = await _context.Guides.Where(s => request.IDs.Contains(s.ID)).ToListAsync();

            if (guide == null)
            {
                return NotFound();
            }

            return guide;
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
                MaxHumidity = request.MaxHumidity ?? 100.0,
                MinHumidity = request.MinHumidity ?? 0.0,
                AirHumidity = request.AirHumidity ?? 0.0,
                SunlightTime = request.SunlightTime ?? 0.0,
                IsPublic = request.IsPublic ?? false,
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

            guideDb.Info = guide.Info ?? guideDb.Info;
            guideDb.MaxHumidity = guide.MaxHumidity ?? guideDb.MaxHumidity;
            guideDb.MinHumidity = guide.MinHumidity ?? guideDb.MinHumidity;
            guideDb.AirHumidity = guide.AirHumidity ?? guideDb.AirHumidity;
            guideDb.SunlightTime = guide.SunlightTime ?? guideDb.SunlightTime;
            guideDb.IsPublic = guide.IsPublic ?? guideDb.IsPublic;

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
