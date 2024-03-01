var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add Azure Queue support to the backend
builder.AddAzureQueueService("queues");

// Add the QueueWorker
builder.Services.AddHostedService<QueueWorker>();

// Add the database context
builder.AddNpgsqlDbContext<TodoDatabaseDbContext>("tododatabase");

// Add services to the container.
builder.Services.AddProblemDetails();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

// Http Api that returns the full list of todos.
app.MapGet("/todos", (TodoDatabaseDbContext todoDatabaseDbContext) => todoDatabaseDbContext.TodoItems.ToArray());

app.MapDefaultEndpoints();

app.Run();

record TodoItem(string Description, bool IsCompleted) { }
