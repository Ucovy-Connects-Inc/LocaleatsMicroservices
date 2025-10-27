using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantService.Data;
using RestaurantService.DTOs;
using RestaurantService.Models;
using RestaurantService.Services;

namespace RestaurantService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RestaurantsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly ICuisineValidationClient _cuisineClient;

    public RestaurantsController(AppDbContext db, ICuisineValidationClient cuisineClient)
    {
        _db = db;
        _cuisineClient = cuisineClient;
    }

    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] int skip = 0, [FromQuery] int limit = 50, [FromQuery] Guid? cuisineId = null)
    {
        var q = _db.Restaurants.AsQueryable();
        if (cuisineId.HasValue) q = q.Where(r => r.CuisineId == cuisineId.Value);
        var items = await q.Skip(skip).Take(limit).ToListAsync();
        return Ok(items.Select(r => new RestaurantReadDto { Id = r.Id, Name = r.Name, CuisineId = r.CuisineId, Address = r.Address, CreatedAt = r.CreatedAt }));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var r = await _db.Restaurants.FindAsync(id);
        if (r == null) return NotFound();
        return Ok(new RestaurantReadDto { Id = r.Id, Name = r.Name, CuisineId = r.CuisineId, Address = r.Address, CreatedAt = r.CreatedAt });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] RestaurantCreateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name)) return BadRequest(new { error = "Name is required" });
        if (dto.CuisineId == Guid.Empty) return BadRequest(new { error = "CuisineId is required" });

        var exists = await _cuisineClient.ExistsAsync(dto.CuisineId);
        if (!exists) return BadRequest(new { error = "CuisineId does not exist" });

        var r = new Restaurant { Id = Guid.NewGuid(), Name = dto.Name.Trim(), CuisineId = dto.CuisineId, Address = dto.Address };
        _db.Restaurants.Add(r);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = r.Id }, new RestaurantReadDto { Id = r.Id, Name = r.Name, CuisineId = r.CuisineId, Address = r.Address, CreatedAt = r.CreatedAt });
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantService.Data;
using RestaurantService.DTOs;
using RestaurantService.Models;
using RestaurantService.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RestaurantsController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly ICuisineClient _cuisineClient;

        public RestaurantsController(AppDbContext db, ICuisineClient cuisineClient)
        {
            _db = db;
            _cuisineClient = cuisineClient;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]int skip = 0, [FromQuery]int limit = 20)
        {
            var items = await _db.Restaurants
                .OrderBy(r => r.CreatedAt)
                .Skip(skip)
                .Take(limit)
                .Select(r => new RestaurantReadDto { Id = r.Id, Name = r.Name, CuisineId = r.CuisineId, CreatedAt = r.CreatedAt })
                .ToListAsync();
            return Ok(items);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var r = await _db.Restaurants.FindAsync(id);
            if (r == null) return NotFound();
            return Ok(new RestaurantReadDto { Id = r.Id, Name = r.Name, CuisineId = r.CuisineId, CreatedAt = r.CreatedAt });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RestaurantCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var exists = await _cuisineClient.CuisineExistsAsync(dto.CuisineId);
            if (!exists)
                return BadRequest(new { error = "CuisineId is invalid" });

            var r = new Restaurant { Id = Guid.NewGuid(), Name = dto.Name, CuisineId = dto.CuisineId };
            _db.Restaurants.Add(r);
            await _db.SaveChangesAsync();
            var read = new RestaurantReadDto { Id = r.Id, Name = r.Name, CuisineId = r.CuisineId, CreatedAt = r.CreatedAt };
            return CreatedAtAction(nameof(GetById), new { id = r.Id }, read);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] RestaurantCreateDto dto)
        {
            var r = await _db.Restaurants.FindAsync(id);
            if (r == null) return NotFound();

            var exists = await _cuisineClient.CuisineExistsAsync(dto.CuisineId);
            if (!exists) return BadRequest(new { error = "CuisineId is invalid" });

            r.Name = dto.Name;
            r.CuisineId = dto.CuisineId;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var r = await _db.Restaurants.FindAsync(id);
            if (r == null) return NotFound();
            _db.Restaurants.Remove(r);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
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
