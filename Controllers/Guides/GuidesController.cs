using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using plant_api.Data;

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

        [HttpGet]
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
        public async Task<ActionResult<Models.Guides>> InsertGuide(Models.Guides guide)
        {
            if (_context.Guides == null)
            {
                return Problem("Entity set 'PlantApiContext.Guides'  is null.");
            }

            guide.ID = await GenerateId();
            _context.Guides.Add(guide);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGuide", new { id = guide.ID }, guide);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGuide(long id, Models.Guides guide)
        {
            if (id != guide.ID)
            {
                return BadRequest();
            }

            _context.Entry(guide).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GuideExists(id))
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
        public async Task<IActionResult> DeleteGuide(long id)
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

            _context.Guides.Remove(guide);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool GuideExists(long id)
        {
            return (_context.Guides?.Any(e => e.ID == id)).GetValueOrDefault();
        }

        private async Task<long> GenerateId()
        {
            try
            {
                if (_context.Guides == null || !_context.Guides.Any())
                    return 1;
                return await _context.Guides.MaxAsync(s => s.ID) + 1;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
