using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CuisineApi.Data;
using CuisineApi.Models;
using CuisineApi.DTOs;


namespace CuisineApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CuisinesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CuisinesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Cuisines
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CuisineReadDto>>> GetAll()
        {
            var list = await _context.Cuisines.AsNoTracking()
                .Select(c => new CuisineReadDto { Id = c.Id, Name = c.Name, Description = c.Description })
                .ToListAsync();

            return Ok(list);
        }

        // GET: api/Cuisines/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CuisineReadDto>> GetById(int id)
        {
            var cuisine = await _context.Cuisines.AsNoTracking()
                .Where(c => c.Id == id)
                .Select(c => new CuisineReadDto { Id = c.Id, Name = c.Name, Description = c.Description })
                .FirstOrDefaultAsync();

            if (cuisine == null) return NotFound();

            return Ok(cuisine);
        }

        // GET: api/Cuisines/{id}/exists
        [HttpGet("{id}/exists")]
        public async Task<IActionResult> Exists(int id)
        {
            var exists = await _context.Cuisines.AsNoTracking().AnyAsync(c => c.Id == id);
            if (exists) return Ok();
            return NotFound();
        }

        // POST: api/Cuisines
        [HttpPost]
        public async Task<ActionResult<CuisineReadDto>> Create([FromBody] CuisineCreateUpdateDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var cuisine = new Cuisine { Name = dto.Name, Description = dto.Description };
            _context.Cuisines.Add(cuisine);
            await _context.SaveChangesAsync();

            var read = new CuisineReadDto { Id = cuisine.Id, Name = cuisine.Name, Description = cuisine.Description };
            return CreatedAtAction(nameof(GetById), new { id = cuisine.Id }, read);
        }

        // PUT: api/Cuisines/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CuisineCreateUpdateDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var cuisine = await _context.Cuisines.FindAsync(id);
            if (cuisine == null) return NotFound();

            cuisine.Name = dto.Name;
            cuisine.Description = dto.Description;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Cuisines.AnyAsync(e => e.Id == id))
                    return NotFound();
                throw;
            }

            return NoContent();
        }

        // DELETE: api/Cuisines/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var cuisine = await _context.Cuisines.FindAsync(id);
            if (cuisine == null) return NotFound();

            _context.Cuisines.Remove(cuisine);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
