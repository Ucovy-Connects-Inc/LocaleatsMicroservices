namespace RestaurantService.DTOs;

public class RestaurantCreateDto
{
    public string Name { get; set; } = string.Empty;
    public Guid CuisineId { get; set; }
    public string? Address { get; set; }
}
using System;
using System.ComponentModel.DataAnnotations;

namespace RestaurantService.DTOs
{
    public class RestaurantCreateDto
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = null!;

        [Required]
        public Guid CuisineId { get; set; }
    }
}
