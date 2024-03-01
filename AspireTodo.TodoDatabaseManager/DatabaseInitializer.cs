
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace AspireTodo.TodoDatabaseManager;

public class DatabaseInitializer(IServiceProvider serviceProvider,
    ILogger<DatabaseInitializer> logger) : BackgroundService
{
    public const string ActivitySourceName = "Migrations";
    private readonly ActivitySource _activitySource = new(ActivitySourceName);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TodoDatabaseDbContext>();

        await InitializeDatabaseAsync(dbContext, stoppingToken);
    }

    private async Task InitializeDatabaseAsync(TodoDatabaseDbContext dbContext, CancellationToken cancellationToken)
    {
        using var activity = _activitySource.StartActivity("Initializing catalog database", ActivityKind.Client);

        var sw = Stopwatch.StartNew();

        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(dbContext.Database.MigrateAsync, cancellationToken);

        await SeedAsync(dbContext, cancellationToken);

        logger.LogInformation("Database initialization completed after {ElapsedMilliseconds}ms", sw.ElapsedMilliseconds);
    }

    private async Task SeedAsync(TodoDatabaseDbContext dbContext, CancellationToken cancellationToken)
    {
        logger.LogInformation("Seeding database");

        var todos = new List<Todo>
        {
            new Todo {  Description = "Build the API", IsCompleted = false },
            new Todo {  Description = "Build the Frontend", IsCompleted = false },
            new Todo {  Description = "Deploy the app", IsCompleted = false }
        };

        if (!dbContext.TodoItems.Any())
        {
            logger.LogInformation("Seeding todo items");
            await dbContext.TodoItems.AddRangeAsync(todos, cancellationToken);
            logger.LogInformation("Seeded todo items");
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}