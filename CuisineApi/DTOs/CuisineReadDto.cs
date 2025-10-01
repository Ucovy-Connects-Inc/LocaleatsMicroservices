namespace CuisineApi.DTOs
{
    public class CuisineReadDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
    }
}
