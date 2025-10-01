using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantService.Data;
using RestaurantService.Models;

namespace RestaurantService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RestaurantsController : ControllerBase
    {
        private readonly AppDbContext _context;
        public RestaurantsController(AppDbContext context) { _context = context; }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Restaurant>>> GetAll() => Ok(await _context.Restaurants.AsNoTracking().ToListAsync());

        [HttpGet("{id}")]
        public async Task<ActionResult<Restaurant>> GetById(int id)
        {
            var r = await _context.Restaurants.FindAsync(id);
            if (r == null) return NotFound();
            return Ok(r);
        }

        [HttpPost]
        public async Task<ActionResult<Restaurant>> Create([FromServices] IHttpClientFactory httpClientFactory, Restaurant restaurant)
        {
            // Validate Cuisine exists via gateway
            try
            {
                var client = httpClientFactory.CreateClient("GatewayClient");
                var resp = await client.GetAsync($"/api/cuisines/{restaurant.CuisineId}/exists");
                if (resp.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return BadRequest($"Cuisine with id {restaurant.CuisineId} does not exist.");
                }
                resp.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                // If gateway is unavailable or other errors, return a 503 to indicate service dependency issue
                return StatusCode(503, $"Unable to validate cuisine: {ex.Message}");
            }

            _context.Restaurants.Add(restaurant);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = restaurant.Id }, restaurant);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Restaurant restaurant)
        {
            if (id != restaurant.Id) return BadRequest();
            _context.Entry(restaurant).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var r = await _context.Restaurants.FindAsync(id);
            if (r == null) return NotFound();
            _context.Restaurants.Remove(r);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
