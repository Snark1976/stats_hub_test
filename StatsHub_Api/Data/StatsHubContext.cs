using Microsoft.EntityFrameworkCore;
using StatsHub_Api.Models;

namespace StatsHub_Api.Data;

public class StatsHubContext(DbContextOptions<StatsHubContext> options) : DbContext(options)
{
    public DbSet<Order> Orders { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>()
            .HasKey(o => o.OrderId);
    }
}