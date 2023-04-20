using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using plant_api.Data;
using plant_api.Helpers;
using plant_api.Models.Review;
using System.Security.Claims;

namespace plant_api.Controllers.Reviews
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly PlantApiContext _context;

        public ReviewsController(PlantApiContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Models.Reviews>>> GetReviews()
        {
          if (_context.Reviews == null)
          {
              return NotFound();
          }
            return await _context.Reviews.ToListAsync();
        }

        [HttpGet("GetMany/{ids}")]
        public async Task<ActionResult<IEnumerable<Models.Devices>>> GetManyGuides(string ids)
        {
            var idsParsed = ids.Split(',');
            long[] idsLong = idsParsed.Select(long.Parse).ToArray();

            if (_context.Reviews == null)
            {
                return NotFound();
            }

            if (idsParsed != null && idsParsed.Length > 0)
            {
                var reviews = await _context.Reviews.Where(d => idsLong.Contains(d.ID)).ToListAsync();
                return Ok(reviews);
            }

            return NotFound();
        }

        [HttpGet("GetByGuideId/{guideId}")]
        public async Task<ActionResult<IEnumerable<Models.Reviews>>> GetReviewsByGuide(long guideId)
        {
            if (_context.Reviews == null)
            {
                return NotFound();
            }

            var reviews = await _context.Reviews.Where(g => g.GuideID == guideId).ToListAsync();

            if (reviews.Any())
            {
                return Ok(reviews);
            }

            return NotFound();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Models.Reviews>> GetReview(long id)
        {
          if (_context.Reviews == null)
          {
              return NotFound();
          }
            var review = await _context.Reviews.FindAsync(id);

            if (review == null)
            {
                return NotFound();
            }

            return review;
        }

        [HttpPost]
        public async Task<ActionResult<Models.Reviews>> PostReview(InsertReviewRequest request)
        {
            var userId = Identity.GetUserId(identity: HttpContext?.User?.Identity as ClaimsIdentity ?? new ClaimsIdentity());

            if (_context.Reviews == null)
            {
                return Problem("Entity set 'PlantApiContext.Reviews'  is null.");
            }

            var review = new Models.Reviews()
            {
                UserID = userId,
                GuideID = request.GuideID,
            };
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetReview", new { id = review.ID }, review);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReview(long id)
        {
            var userId = Identity.GetUserId(identity: HttpContext?.User?.Identity as ClaimsIdentity ?? new ClaimsIdentity());

            if (_context.Reviews == null)
            {
                return NotFound();
            }

            var review = await _context.Reviews.FirstOrDefaultAsync(d => d.ID == id && d.UserID == userId);

            if (review == null)
            {
                return NotFound();
            }

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("ForceDelete/{id}")]
        public async Task<IActionResult> ForceDeleteReview(long id)
        {
            if (_context.Reviews == null)
            {
                return NotFound();
            }
            var review = await _context.Reviews.FindAsync(id);
            if (review == null)
            {
                return NotFound();
            }

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
