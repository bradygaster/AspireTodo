var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

// We'll replace this with a database later.
List<TodoItem> todos = new()
{
    new TodoItem("Build the API", false),
    new TodoItem("Build the Frontend", false),
    new TodoItem("Deploy the app", false),
};

// Http Api that returns the full list of todos.
app.MapGet("/todos", () => todos);

app.MapDefaultEndpoints();

app.Run();

record TodoItem(string Description, bool IsCompleted) { }
