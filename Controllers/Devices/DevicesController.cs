﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using plant_api.Data;
using plant_api.Controllers.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;
using plant_api.Helpers;
using plant_api.Models.Device;
using System.Linq.Dynamic.Core;

namespace plant_api.Controllers.Devices
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Models.Devices>>> GetDevice()
        {
            var userId = Identity.GetUserId(identity: HttpContext?.User?.Identity as ClaimsIdentity ?? new ClaimsIdentity());

            if (_context.Devices == null)
            {
                return NotFound();
            }

            var devices = await _context.Devices.Where(d => d.UserID == userId).ToListAsync();

            return Ok(devices);
        }

        [HttpGet("GetList/{props}")]
        public async Task<ActionResult<IEnumerable<Models.Devices>>> GetDevicesList(string props)
        {
            if (props == null)
            {
                return BadRequest();
            }

            if (_context.Devices == null)
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
                var species = _context.Devices
                    .OrderBy("s=>s." + sortField)
                    .AsEnumerable()
                    .Where((s, i) => i >= (pageInt - 1) * perPageInt && i < pageInt * perPageInt)
                    .ToList();
                return Ok(species);
            }
            else
            {
                var species = _context.Devices
                    .OrderBy("s=>s." + sortField + " DESC")
                    .AsEnumerable()
                    .Where((s, i) => i >= (pageInt - 1) * perPageInt && i < pageInt * perPageInt)
                    .ToList();
                return Ok(species);
            }
        }

        [HttpGet("GetByName/{name}/{props}")]
        public async Task<ActionResult<IEnumerable<Models.Devices>>> GetDevicesByName(string name, string props)
        {
            if (props == null)
            {
                return BadRequest();
            }

            if (_context.Devices == null)
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
                var device = _context.Devices
                    .OrderBy("s=>s." + sortField)
                    .AsEnumerable()
                    .Where((s, i) => i >= (pageInt - 1) * perPageInt && i < pageInt * perPageInt && s.Name.Contains(name))
                    .ToList();
                return Ok(device);
            }
            else
            {
                var device = _context.Devices
                    .OrderBy("s=>s." + sortField + " DESC")
                    .AsEnumerable()
                    .Where((s, i) => i >= (pageInt - 1) * perPageInt && i < pageInt * perPageInt && s.Name.Contains(name))
                    .ToList();
                return Ok(device);
            }
        }

        [HttpGet("GetMany/{ids}")]
        public async Task<ActionResult<IEnumerable<Models.Devices>>> GetManyDevices(string ids)
        {
            var userId = Identity.GetUserId(identity: HttpContext?.User?.Identity as ClaimsIdentity ?? new ClaimsIdentity());

            var idsParsed = ids.Split(',');
            long[] idsLong = idsParsed.Select(long.Parse).ToArray();

            if (_context.Devices == null)
            {
                return NotFound();
            }

            if (idsParsed != null && idsParsed.Length > 0)
            {
                var devices = await _context.Devices.Where(d => d.UserID == userId && idsLong.Contains(d.ID)).ToListAsync();
                return Ok(devices);
            }

            return NotFound();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Models.Devices>> GetDevice(long id)
        {
            var userId = Identity.GetUserId(identity: HttpContext?.User?.Identity as ClaimsIdentity ?? new ClaimsIdentity());

            if (_context.Devices == null)
            {
                return NotFound();
            }

            var device = await _context.Devices.FirstOrDefaultAsync(d => d.ID == id && d.UserID == userId);

            if (device == null)
            {
                return NotFound();
            }

            return Ok(device);
        }

        [HttpPost]
        public async Task<ActionResult<Models.Devices>> InsertDevice(InsertDeviceRequest request)
        {
            var userId = Identity.GetUserId(identity: HttpContext?.User?.Identity as ClaimsIdentity ?? new ClaimsIdentity());

            if (_context.Devices == null)
            {
                return Problem("Entity set 'PlantApiContext.Device'  is null.");
            }

            var device = new Models.Devices()
            {
                UserID = userId,
                PlantID = request.PlantID,
                Name = request.Name,
            };
            _context.Devices.Add(device);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDevice", new { id = device.ID }, device);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDevice(long id, UpdateDeviceRequest device)
        {
            var userId = Identity.GetUserId(identity: HttpContext?.User?.Identity as ClaimsIdentity ?? new ClaimsIdentity());

            if (_context.Devices == null)
            {
                return NotFound();
            }

            var deviceDb = await _context.Devices.FirstOrDefaultAsync(d => d.ID == id && d.UserID == userId);

            if (deviceDb == null)
            {
                return NotFound();
            }

            deviceDb.Name = device.Name ?? deviceDb.Name;
            deviceDb.PlantID = device.PlantID ?? deviceDb.PlantID;

            _context.Entry(deviceDb).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(deviceDb);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DeviceExists(id))
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
        public async Task<IActionResult> DeleteDevice(long id)
        {
            var userId = Identity.GetUserId(identity: HttpContext?.User?.Identity as ClaimsIdentity ?? new ClaimsIdentity());

            if (_context.Devices == null)
            {
                return NotFound();
            }

            var device = await _context.Devices.FirstOrDefaultAsync(d => d.ID == id && d.UserID == userId);

            if (device == null)
            {
                return NotFound();
            }

            _context.Devices.Remove(device);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("ForceDelete/{id}")]
        public async Task<IActionResult> ForceDeleteDevice(long id)
        {
            if (_context.Devices == null)
            {
                return NotFound();
            }

            var device = await _context.Devices.FirstOrDefaultAsync(d => d.ID == id);
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
    }
}
