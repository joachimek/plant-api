using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using plant_api.Data;

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
        public async Task<ActionResult<IEnumerable<Models.Species>>> GetSpecies()
        {
          if (_context.Species == null)
          {
              return NotFound();
          }
            return await _context.Species.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Models.Species>> GetSpecies(long id)
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

        [HttpPut("{id}")]
        public async Task<IActionResult> PutSpecies(long id, Models.Species species)
        {
            if (id != species.ID)
            {
                return BadRequest();
            }

            _context.Entry(species).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SpeciesExists(id))
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
        public async Task<ActionResult<Models.Species>> PostSpecies(Models.Species species)
        {
          if (_context.Species == null)
          {
              return Problem("Entity set 'PlantApiContext.Species'  is null.");
          }
            species.ID = await GenerateId();
            _context.Species.Add(species);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSpecies", new { id = species.ID }, species);
        }

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

        private async Task<long> GenerateId()
        {
            try
            {
                if (!_context.Species.Any())
                    return 1;
                return await _context.Species?.MaxAsync(s => s.ID) + 1;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
