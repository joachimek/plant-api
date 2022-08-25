using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using plant_api.Data;
using plant_api.Helpers;
using plant_api.Models.User;

namespace plant_api.Controllers.Users
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly PlantApiContext _context;

        public UsersController(PlantApiContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Models.Users>>> GetUsers()
        {
          if (_context.Users == null)
          {
              return NotFound();
          }
            return await _context.Users.ToListAsync();
        }

        [HttpGet("{username}")]
        public async Task<ActionResult<Models.Users>> GetUser(long username)
        {
          if (_context.Users == null)
          {
              return NotFound();
          }
            var user = await _context.Users.FindAsync(username);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        [HttpPost]
        public async Task<ActionResult<Models.Users>> InsertUser(InsertUserRequest request)
        {
            if (_context.Users == null)
            {
              return Problem("Entity set 'PlantApiContext.Users'  is null.");
            }

            var user = new Models.Users() 
            { 
                Password = Cryptography.MD5Hash(request.Password), 
                Username = request.Username, 
                EmailAddress = request.EmailAddress, 
                Role = request.Role, 
                Devices = new List<Models.Devices>() 
            };   
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { username = request.Username }, request);
        }

        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateUser(string userId, UpdateUserRequest request)
        {
            if (_context.Users == null)
            {
                return NotFound();
            }

            if (!long.TryParse(userId, out var userIdParsed))
            {
                return NotFound();
            }

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userIdParsed);

            if (user == null)
            {
                return NotFound();
            }

            user.Username = request.Username;
            user.EmailAddress = request.EmailAddress;
            user.Role = request.Role;

            _context.Entry(user).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return CreatedAtAction("UpdateUser", new { id = userId }, request);
        }
    }
}
