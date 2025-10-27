using System;
using System.Threading.Tasks;

namespace RestaurantService.Services
{
    public interface ICuisineClient
    {
        Task<bool> CuisineExistsAsync(Guid cuisineId);
    }
}
