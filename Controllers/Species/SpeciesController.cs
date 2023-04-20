using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using plant_api.Data;
using plant_api.Models.Species;

namespace plant_api.Controllers.Species
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class SpeciesController : ControllerBase
    {
        private readonly PlantApiContext _context;

        public SpeciesController(PlantApiContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Models.SpeciesDto>>> GetSpecies()
        {
            if (_context.Species == null)
            {
                return NotFound();
            }
            return await _context.Species.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Models.SpeciesDto>> GetSpecies(long id)
        {
            if (_context.Species == null)
            {
                return NotFound();
            }
            var species = await _context.Species.FindAsync(id);

            if (species == null)
            {
                return NotFound();
            }

            return species;
        }

        [HttpPost("GetMany")]
        public async Task<ActionResult<IEnumerable<Models.SpeciesDto>>> GetMany(Models.Common.GetManyRequest request)
        {
            if (_context.Species == null)
            {
                return NotFound();
            }
            var species = await _context.Species.Where(s => request.IDs.Contains(s.ID)).ToListAsync();

            if (species == null)
            {
                return NotFound();
            }

            return species;
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<ActionResult<Models.SpeciesDto>> InsertSpecies(InsertSpeciesRequest request)
        {
            if (_context.Species == null)
            {
                return Problem("Entity set 'PlantApiContext.Species'  is null.");
            }

            var species = new Models.SpeciesDto()
            {
                Name = request.Name,
                Info = request.Info,
                IsPublic = request.IsPublic,
            };
            _context.Species.Add(species);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSpecies", new { id = species.ID }, species);
        }

        [Authorize(Roles = "admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSpecies(long id, UpdateSpeciesRequest species)
        {
            if (_context.Species == null)
            {
                return NotFound();
            }

            var speciesDb = await _context.Species.FirstOrDefaultAsync(d => d.ID == id);

            if (speciesDb == null)
            {
                return NotFound();
            }

            speciesDb.Name = species.Name ?? speciesDb.Name;
            speciesDb.Info = species.Info ?? speciesDb.Info;
            speciesDb.IsPublic = species.IsPublic ?? speciesDb.IsPublic;

            _context.Entry(speciesDb).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(speciesDb);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SpeciesExists(id))
                {
                    return NotFound();
                }
                else
                {
                    return NoContent();
                }
            }
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSpecies(long id)
        {
            if (_context.Species == null)
            {
                return NotFound();
            }
            var species = await _context.Species.FindAsync(id);
            if (species == null)
            {
                return NotFound();
            }

            _context.Species.Remove(species);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SpeciesExists(long id)
        {
            return (_context.Species?.Any(e => e.ID == id)).GetValueOrDefault();
        }
    }
}
