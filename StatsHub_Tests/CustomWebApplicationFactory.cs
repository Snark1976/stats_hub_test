using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StatsHub_Api;
using StatsHub_Api.Data;

namespace StatsHub.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = $"StatsHubTestDb_{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            var contextDescriptors = services
                .Where(d => d.ServiceType == typeof(DbContextOptions<StatsHubContext>) ||
                            d.ServiceType == typeof(StatsHubContext))
                .ToList();

            foreach (var descriptor in contextDescriptors)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<StatsHubContext>(options =>
                options.UseInMemoryDatabase(_databaseName));

            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<StatsHubContext>();
            db.Database.EnsureCreated();
        });
    }
}
