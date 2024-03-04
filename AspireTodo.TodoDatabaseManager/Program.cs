using AspireTodo.TodoDatabaseManager;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add the database context
builder.AddNpgsqlDbContext<TodoDatabaseDbContext>("tododatabase", null,
    optionsBuilder => optionsBuilder.UseNpgsql(npgsqlBuilder =>
        npgsqlBuilder.MigrationsAssembly(typeof(Program).Assembly.GetName().Name)));

// Add OTel, and wire up the database initialization's "migration" activity
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource(DatabaseInitializer.ActivitySourceName));

// Add the database initialization service as a background worker
builder.Services.AddSingleton<DatabaseInitializer>();
builder.Services.AddHostedService(sp => sp.GetRequiredService<DatabaseInitializer>());

var app = builder.Build();

app.MapDefaultEndpoints();

app.Run();

