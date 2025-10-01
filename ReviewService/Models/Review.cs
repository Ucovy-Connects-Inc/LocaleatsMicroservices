namespace ReviewService.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int RestaurantId { get; set; }
        public int UserId { get; set; }
        public int Rating { get; set; }
        public string? Text { get; set; }
    }
}
