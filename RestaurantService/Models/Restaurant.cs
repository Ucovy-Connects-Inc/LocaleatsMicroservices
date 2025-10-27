namespace RestaurantService.Models;

public class Restaurant
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid CuisineId { get; set; }
    public string? Address { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
using System;

namespace RestaurantService.Models
{
    public class Restaurant
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public Guid CuisineId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
namespace RestaurantService.Models
{
    public class Restaurant
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public int CuisineId { get; set; }
        public string? Address { get; set; }
    }
}
