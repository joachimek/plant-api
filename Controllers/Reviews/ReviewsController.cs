using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using plant_api.Data;

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

        [HttpGet]
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

        [HttpPut("{id}")]
        public async Task<IActionResult> PutReview(long id, Models.Reviews review)
        {
            if (id != review.ID)
            {
                return BadRequest();
            }

            _context.Entry(review).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReviewExists(id))
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
        public async Task<ActionResult<Models.Reviews>> PostReview(Models.Reviews review)
        {
          if (_context.Reviews == null)
          {
              return Problem("Entity set 'PlantApiContext.Reviews'  is null.");
          }
            review.ID = await GenerateId();
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetReview", new { id = review.ID }, review);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReview(long id)
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

        private bool ReviewExists(long id)
        {
            return (_context.Reviews?.Any(e => e.ID == id)).GetValueOrDefault();
        }

        private async Task<long> GenerateId()
        {
            try
            {
                if (_context.Reviews == null || !_context.Reviews.Any())
                    return 1;
                return await _context.Reviews.MaxAsync(s => s.ID) + 1;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
