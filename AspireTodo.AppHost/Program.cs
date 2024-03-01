var builder = DistributedApplication.CreateBuilder(args);

var tododatabase = builder.AddPostgresContainer("postgres").AddDatabase("tododatabase");

var storage = builder.AddAzureStorage("storage").UseEmulator();

var queues = storage.AddQueues("queues");

var apiService = builder.AddProject<Projects.AspireTodo_ApiService>("apiservice")
    .WithReference(queues)
    .WithReference(tododatabase);

var frontend = builder.AddProject<Projects.AspireTodo_Web>("webfrontend")
    .WithReference(queues)
    .WithReference(apiService);

builder.AddProject<Projects.AspireTodo_TodoDatabaseManager>("tododatabasemanager")
    .WithReference(tododatabase);

builder.Build().Run();
