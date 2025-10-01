using System.ComponentModel.DataAnnotations;

namespace CuisineApi.DTOs
{
    public class CuisineCreateUpdateDto
    {
        [Required]
        [StringLength(200, MinimumLength = 1)]
        public string Name { get; set; } = default!;

        [StringLength(1000)]
        public string? Description { get; set; }
    }
}
