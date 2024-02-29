using Microsoft.Extensions.Caching.Memory;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add memory caching to store the todos on the server for now
builder.Services.AddMemoryCache();

// Add Azure Queue support to the backend
builder.AddAzureQueueService("queues");

// Add the QueueWorker
builder.Services.AddHostedService<QueueWorker>();

// Add services to the container.
builder.Services.AddProblemDetails();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

app.Services.GetRequiredService<IMemoryCache>().Set("todos", new List<TodoItem>
{
    new TodoItem("Build the API", false),
    new TodoItem("Build the Frontend", false),
    new TodoItem("Deploy the app", false),
});

// Http Api that returns the full list of todos.
app.MapGet("/todos", (IMemoryCache memoryCache) => memoryCache.Get<List<TodoItem>>("todos"));

app.MapDefaultEndpoints();

app.Run();

record TodoItem(string Description, bool IsCompleted) { }
