using Microsoft.EntityFrameworkCore;
using CuisineApi.Data;

namespace CuisineApi.Tests.TestHelpers
{
    public static class DbContextFactory
    {
        public static AppDbContext CreateInMemoryContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;

            var context = new AppDbContext(options);
            context.Database.EnsureCreated();
            return context;
        }
    }
}
