namespace RestaurantService.Services;

public interface ICuisineValidationClient
{
    Task<bool> ExistsAsync(Guid cuisineId);
}
