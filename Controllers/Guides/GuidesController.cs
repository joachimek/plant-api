using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using plant_api.Data;
using plant_api.Models;

namespace plant_api.Controllers.Guides
{
    [Route("api/[controller]")]
    [ApiController]
    public class GuidesController : ControllerBase
    {
        private readonly PlantApiContext _context;

        public GuidesController(PlantApiContext context)
        {
            _context = context;
        }

        // GET: api/Guides
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Models.Guides>>> GetGuides()
        {
          if (_context.Guides == null)
          {
              return NotFound();
          }
            return await _context.Guides.ToListAsync();
        }

        // GET: api/Guides/5
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

            return guide;
        }

        // PUT: api/Guides/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGuide(long id, Models.Guides guide)
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

        // POST: api/Guides
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Models.Guides>> PostGuide(Models.Guides guide)
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

        // DELETE: api/Guides/5
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
                if (!_context.Guides.Any())
                    return 1;
                return await _context.Guides?.MaxAsync(s => s.ID) + 1;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
