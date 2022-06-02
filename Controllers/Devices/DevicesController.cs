using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using plant_api.Data;
using plant_api.Models;
using plant_api.Controllers.Common;

namespace plant_api.Controllers.Devices
{
    [Route("api/[controller]")]
    [ApiController]
    public class DevicesController : ControllerBase
    {
        private readonly PlantApiContext _context;

        public DevicesController(PlantApiContext context)
        {
            _context = context;
        }

        public SortPayload sort { get; set; } = new SortPayload();
        public string? DateSort { get; set; }
        public string? CurrentFilter { get; set; }
        public string? CurrentSort { get; set; }

        public IList<Models.Devices>? Devices { get; set; }

        // GET: api/Devices
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Models.Devices>>> GetDevice()
        {
          if (_context.Devices == null)
          {
              return NotFound();
          }
            return await _context.Devices.ToListAsync();
        }

        // GET: api/Devices/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Models.Devices>> GetDevice(long id)
        {
          if (_context.Devices == null)
          {
              return NotFound();
          }
            var device = await _context.Devices.FindAsync(id);

            if (device == null)
            {
                return NotFound();
            }

            return device;
        }

        // PUT: api/Devices/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDevice(long id, Models.Devices device)
        {
            if (id != device.ID)
            {
                return BadRequest();
            }

            _context.Entry(device).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DeviceExists(id))
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

        // POST: api/Devices
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Models.Devices>> PostDevice(Models.Devices device)
        {
          if (_context.Devices == null)
          {
              return Problem("Entity set 'PlantApiContext.Device'  is null.");
          }
            device.ID = await GenerateId();
            _context.Devices.Add(device);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDevice", new { id = device.ID }, device);
        }

        // DELETE: api/Devices/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDevice(long id)
        {
            if (_context.Devices == null)
            {
                return NotFound();
            }
            var device = await _context.Devices.FindAsync(id);
            if (device == null)
            {
                return NotFound();
            }

            _context.Devices.Remove(device);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DeviceExists(long id)
        {
            return (_context.Devices?.Any(e => e.ID == id)).GetValueOrDefault();
        }

        private async Task<long> GenerateId()
        {
            try
            {
                if (!_context.Devices.Any())
                    return 1;
                return await _context.Devices?.MaxAsync(s => s.ID) + 1;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
