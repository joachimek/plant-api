using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using plant_api.Data;
using plant_api.Models;
using plant_api.Models.Species;
using System.Data.Entity;
using System.Linq.Dynamic.Core;

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
            return _context.Species.Where(s => s.IsPublic == true).ToList();
        }

        [HttpGet("GetList/{props}")]
        public async Task<ActionResult<IEnumerable<Models.SpeciesDto>>> GetSpeciesList(string props)
        {
            if (props == null)
            {
                return BadRequest();
            }

            if (_context.Species == null)
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
                var species = _context.Species
                    .OrderBy("s=>s." + sortField)
                    .AsEnumerable()
                    .Where((s, i) => s.IsPublic == true && i >= (pageInt - 1) * perPageInt && i < pageInt * perPageInt)
                    .ToList();
                return Ok(species);
            }
            else
            {
                var species = _context.Species
                    .OrderBy("s=>s." + sortField + " DESC")
                    .AsEnumerable()
                    .Where((s, i) => s.IsPublic == true && i >= (pageInt - 1) * perPageInt && i < pageInt * perPageInt)
                    .ToList();
                return Ok(species);
            }
        }
         
        [HttpGet("GetByName/{name}/{props}")]
        public async Task<ActionResult<IEnumerable<Models.SpeciesDto>>> GetSpeciesByName(string name, string props)
        {
            if (props == null)
            {
                return BadRequest();
            }

            if (_context.Species == null)
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

            if (sortOrder == "ASC")
            {
                var species = _context.Species.Where(s => s.IsPublic == true && s.Name.Contains(name)).OrderBy(sortField).ToList();
                return Ok(species);
            }
            else
            {
                var species = _context.Species.Where(s => s.IsPublic == true && s.Name.Contains(name)).OrderBy(sortField + " descending").ToList();
                return Ok(species);
            }
        }

        [HttpGet("GetMany/{ids}")]
        public async Task<ActionResult<IEnumerable<Models.SpeciesDto>>> GetManySpecies(string ids)
        {
            var idsParsed = ids.Split(',');
            long[] idsLong = idsParsed.Select(long.Parse).ToArray();

            if (_context.Species == null)
            {
                return NotFound();
            }

            if (idsParsed != null && idsParsed.Length > 0)
            {
                var species = _context.Species.Where(d => idsLong.Contains(d.ID)).ToList();
                return Ok(species);
            }

            return NotFound();
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
        
        [HttpGet("GetByName/{name}")]
        public async Task<ActionResult<IEnumerable<Models.SpeciesDto>>> GetSpeciesByName(string name)
        {
            if (_context.Species == null)
            {
                return NotFound();
            }

            var species = _context.Species.Where(s => s.Name.Contains(name)).ToList();

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

            _context.Entry(speciesDb).State = Microsoft.EntityFrameworkCore.EntityState.Modified;

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
