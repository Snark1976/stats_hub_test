using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Text.Json;
using System.Text.Json.Serialization;
using StatsHub_Api.Data;
using StatsHub_Api.Services;

namespace StatsHub_Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Host.UseSerilog((ctx, lc) => lc
            .WriteTo.Console()
            .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day));

        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowWeb", policy =>
            {
                policy.WithOrigins("http://localhost:8081")
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });

        bool isNotTesting = !builder.Environment.IsEnvironment("Testing");
        if (isNotTesting)
        {
            builder.Services.AddDbContext<StatsHubContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddScoped<DatabaseMigrator>();
        }

        builder.Services.AddMemoryCache();
        builder.Services.AddScoped<OrderService>();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new() { Title = "StatsHub API", Version = "v1" });
        });

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "StatsHub API v1"));
        }

        app.UseSerilogRequestLogging();
        app.UseHttpsRedirection();

        app.UseCors("AllowWeb");

        app.UseAuthorization();
        app.MapControllers();

        if (isNotTesting)
        {
            using var scope = app.Services.CreateScope();
            var migrator = scope.ServiceProvider.GetRequiredService<DatabaseMigrator>();
            migrator.ApplyMigrations();
        }

        app.Run();
    }
}