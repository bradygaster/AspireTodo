var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.AspireTodo_ApiService>("apiservice");

builder.AddProject<Projects.AspireTodo_Web>("webfrontend")
    .WithReference(apiService);

builder.Build().Run();
