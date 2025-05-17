using Microsoft.EntityFrameworkCore;
using StatsHub.Api.Models;

namespace StatsHub.Api.Data;

public class StatsHubContext(DbContextOptions<StatsHubContext> options) : DbContext(options)
{
    public DbSet<Order> Orders { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>()
            .HasKey(o => o.OrderId);
    }
}