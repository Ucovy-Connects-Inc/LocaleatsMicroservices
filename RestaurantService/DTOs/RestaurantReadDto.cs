namespace RestaurantService.DTOs;

public class RestaurantReadDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid CuisineId { get; set; }
    public string? Address { get; set; }
    public DateTime CreatedAt { get; set; }
}
using System;

namespace RestaurantService.DTOs
{
    public class RestaurantReadDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public Guid CuisineId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
