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
