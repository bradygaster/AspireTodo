var builder = DistributedApplication.CreateBuilder(args);

var storage = builder.AddAzureStorage("storage").UseEmulator();

var queues = storage.AddQueues("queues");

var apiService = builder.AddProject<Projects.AspireTodo_ApiService>("apiservice")
    .WithReference(queues);

var frontend = builder.AddProject<Projects.AspireTodo_Web>("webfrontend")
    .WithReference(queues)
    .WithReference(apiService);

builder.Build().Run();
