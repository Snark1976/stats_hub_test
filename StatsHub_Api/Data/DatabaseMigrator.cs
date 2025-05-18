using Microsoft.EntityFrameworkCore;

namespace StatsHub_Api.Data;

public class DatabaseMigrator(StatsHubContext dbContext, ILogger<DatabaseMigrator> logger)
{
    private readonly StatsHubContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

    public void ApplyMigrations()
    {
        logger.LogInformation("Checking database connection...");
        for (int attempt = 1; attempt <= 5; attempt++)
        {
            try
            {
                if (_dbContext.Database.CanConnect())
                {
                    logger.LogInformation("Database connection established.");
                    break;
                }
                logger.LogInformation($"Database not ready, attempt {attempt}/5. Retrying in 5 seconds...");
                Thread.Sleep(5000);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, $"Connection attempt {attempt}/5 failed.");
                if (attempt == 5) throw;
                Thread.Sleep(5000);
            }
        }

        logger.LogInformation("Starting database migrations...");
        _dbContext.Database.Migrate();
        logger.LogInformation("Database migrations applied successfully.");
    }
}