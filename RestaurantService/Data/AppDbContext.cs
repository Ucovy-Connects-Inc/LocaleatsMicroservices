using Microsoft.EntityFrameworkCore;
using RestaurantService.Models;

namespace RestaurantService.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Restaurant> Restaurants { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Restaurant>(b =>
        {
            b.HasKey(r => r.Id);
            b.Property(r => r.Name).IsRequired().HasMaxLength(200);
            b.Property(r => r.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
        });
    }
}
using Microsoft.EntityFrameworkCore;
using RestaurantService.Models;

namespace RestaurantService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Restaurant> Restaurants { get; set; }
    }
}
using Microsoft.EntityFrameworkCore;
using RestaurantService.Models;

namespace RestaurantService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Restaurant> Restaurants { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Restaurant>(b =>
            {
                b.HasKey(x => x.Id);
                b.Property(x => x.Name).IsRequired().HasMaxLength(300);
            });
        }
    }
}
