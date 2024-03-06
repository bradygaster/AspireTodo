# AspireTodo

This repo contains the sample code we'll use for the .NET + Azure (via Aspire) training day. 

> Note, I'm just iterating over the steps in here quickly and will go back and refine. If you look at the repo's commit history you'll see these steps embodied, commit-by-commit, in the actual source code for the app. 

## Day 1

These are the high-level steps you'll perform on Day 1. 

### Creating a new Aspire project

You'l create a basic project and then learn how to run it and examine how the app is running using the .NET Aspire dashboard. 

* Create a new Aspire Starter project with the Redis Output Caching enabled 
* Run the app to see the Aspire Dashboard 
* Run through the various ways of looking at console logs for each project 
* Observe how the Trace node provides distributed tracing as users hit the frontend of the site
* Stop the debugger

Next, you'll start editing the code to turn it into your very own Todo app. 

---

### Change the code to be a "Todo" app

In this phase, you'll make some basic modifications to the backend API and frontend Web project to turn the template's content into a real Todo app. 

* In the `ApiService` project's `Program.cs`, delete the `summaries` variable, as well as the sole call to `app.MapGet`, and delete the `WeatherForecast` C# `record` from the class to remove the template's sample code
* Add this code to create a new `TodoItem` type at the bottom of the `ApiService` project's `Program.cs` file:

  ```csharp
  // how the API models a TodoItem object
  record TodoItem(string Description, bool IsCompleted) { }
  ```

* Add code before the `app.MapDefaultEndpoints()` call in the `ApiService` project's `Program.cs` to build a static list of `TodoItem` instances and return them via an HTTP endpoint:

  ```csharp
  // A static list of TodoItems to get us started
  List<TodoItem> todoItems = new List<TodoItem>
  {
      new("Build the API", false),
      new("Build the Frontend", false),
      new("Deploy the app", false)
  };

  // Http Api that returns the full list of todos.
  app.MapGet("/todos", () => todoItems);
  ```

* Rename the `Web` project's `WeatherApiClient.cs`, to `TodoApiClient.cs`, and change the code in the file to be this code:

  ```csharp
  namespace AspireTodo.Web;

  public class TodoApiClient(HttpClient httpClient)
  {
      public async Task<TodoItem[]> GetAllTodoItems()
      {
          return await httpClient.GetFromJsonAsync<TodoItem[]>("/todos") ?? [];
      }
  }

  public record TodoItem(string Description, bool IsCompleted) { }

  ```

* Reflect the type name change in the `Web` project's `Program.cs` during build-up by changing this line:

  ```csharp
  builder.Services.AddHttpClient<WeatherApiClient>(client => client.BaseAddress = new("http://apiservice"));
  ```

  to this:

  ```csharp
  builder.Services.AddHttpClient<TodoApiClient>(client => client.BaseAddress = new("http://apiservice"));
  ```

* Rename the `Web` project's `Weather.razor` in the `Pages` directory to `Todo.razor` and change the code in the resulting `Todo.razor` from this:

  ```csharp
  @page "/weather"
  @attribute [StreamRendering(true)]
  @attribute [OutputCache(Duration = 5)]

  @inject TodoApiClient WeatherApi
  ```

  to this:

  ```csharp
  @page "/"
  @rendermode InteractiveServer
  @inject TodoApiClient TodoApiClient
  @using AspireTodo.Web
  ```

* Change the `Web` project's `Layout/NavMenu.razor` so that it only has the `Home` link, deleting `Weather` and `Counter`, but change the link text to `Todo`:

  ```html
  <div class="top-row ps-3 navbar navbar-dark">
      <div class="container-fluid">
          <a class="navbar-brand" href="">AspireTodo</a>
      </div>
  </div>

  <input type="checkbox" title="Navigation menu" class="navbar-toggler" />

  <div class="nav-scrollable" onclick="document.querySelector('.navbar-toggler').click()">
      <nav class="flex-column">
          <div class="nav-item px-3">
              <NavLink class="nav-link" href="" Match="NavLinkMatch.All">
                  <span class="bi bi-house-door-fill" aria-hidden="true"></span> Todo
              </NavLink>
          </div>
      </nav>
  </div>

  ```

* Edit the `Web` project's `Pages/Todo.razor` file to display `TodoItem` objects instead of the old `WeatherForecast` objects, using the `TodoApiClient` class instead of the `WeatherApiClient` class:

  ```html
  <PageTitle>AspireTodo</PageTitle>

  <h1>Todo</h1>

  <p>Below are all of the items you AspireTodo.</p>

  @if (todos == null)
  {
      <p><em>Loading...</em></p>
  }
  else
  {
      <table class="table">
          <thead>
              <tr>
                  <th>Todo</th>
              </tr>
          </thead>
          <tbody>
              @foreach (var todo in todos)
              {
                  <tr>
                      <td>@todo.Description</td>
                  </tr>
              }
          </tbody>
      </table>
  }

  @code {
      private TodoItem[]? todos;

      protected override async Task OnInitializedAsync()
      {
          todos = await TodoApiClient.GetAllTodoItems();
      }
  }
  ```

* Delete the `Counter.razor` and `Home.razor` files from the `Web` project's `Pages` folder

With these changes made, you're ready to deploy the app right up to Azure to get started learning the platform's components. 

---

### Deploy the app to Azure

In this phase, you'll publish your new AspireTodo app to Azure using the Azure Developer CLI or, Visual Studio (which uses the Azure Developer CLI as an underlying dependency). 

* Make sure AZD is defaulting to the .NET R&D subscription so you know you have everything you need to party today

    ```azd config set defaults.subscription <.NETR&DSubscriptionIdHere>```

* If you had Visual Studio open when you performed the `azd config set`, you may need to restart Visual Studio
* Right-click the `AppHost` project and select `Publish`, then go through the .NET Aspire Azure Container Apps publish flow to publish the app to Azure 
* If you aren't using Visual Studio, you can simply drop out a command prompt, `cd` into the root directory of your `.sln` file, and execute these commands:

    ```
    azd init
    azd provision
    azd deploy
    ```

* Browse to the Azure portal and see the variety of resources you've created
* Click on the `webfrontend` Azure Container App resource to view the overview of the app 
* Explore the `Ingress` area for both the `backendapi` and `webfrontend` container apps to take note of how the frontend is available via the open Internet, whereas the backend API app is private and secure

With the app published manually, you're ready to save your code and get ready for Day 2. 

---

### Push your app into a GitHub repo

At this point your code should be ready to save, so get it into a GitHub repo (public or private, doesn't matter).

* Use Visual Studio's GitHub features, the `gh` CLI tool, or your favorite method of creating Git repositories to create a new local Git repository and remote GitHub repository
* Commit and push your code to the remote repository

This should be the end of the stuff we get through in Day 1. We'll kick off Day 2 by automating the deployment of your app when you commit code to the repo in which you've saved your code. 

## Day 2

During Day 2, you'll add Continuous Integration / Continuous Deploy (CI/CD) capabilities to your GitHub repository, add database support, and use asynchrous messaging rather than direct HTTP calls between your frontend and backend apps. 

### Automate dotnet build

In this phase you'll automate the process of building the `AspireTodo` source code whenever you want, or, whenever you commit. 

* Open your GitHub AspireTodo repository and go to the `Actions` tab
* Search for the `Continuous integration` area
* Find the `.NET` item in this section, labelled `Build and test a .NET or ASP.NET Core project` and click the `Configure` button
* Change the `checkout` and `setup-dotnet` tasks to be `v4`
* Change the `dotnet-version` from `6.0.x` to `8.0.102`
* Add a step between the change you just made and the `Restore dependencies` step containing this YAML code

    ```yaml
    - name: Install workload
      run: dotnet workload install aspire
    ```

* Commit the YAML file back to your `main` branch once you've made these changes
* Browse to the `Actions` tab in GitHub and watch your continuous integration build your app

At this point, make sure you clone your changes back to your DevBox or Virtual Machine, so you have the changes you just made in the browser back down on your workstation. 

> Note: You may have changes locally in addition to the change to activate continuous integration. If so, add and commit the `azure.yaml` file, and add the `.azure` folder to your `.gitignore` file. Then perform a commit-and-push and then a pull to synchronize the local changes with the remote changes. 

---

### Setting up Continuous Deployment

Since you already have an AZD environment provisioned in Azure *and* the local configuration specifying that AZD environment as the destination to which code should be deployed when changes happen, and since AZD works the same in CI/CD as it does locally, the process is simple. 

* At the command line, type the command

    ```
    azd pipeline config --auth-type client-credentials --provider github --principal-name augmentrprincipal --environment <yourEnvironmentNameHere>
    ```

    > Note: The parameter `augmentrprincipal` is a managed identity created for another sample, but it'll work here, too. Since not everyone in the class may have (or need) the permissions required to create Managed Identities in the R&D sub, we'll just use this one for now.

* Go to your GitHub repositorie's `Settings` area, and you'll notice that both secrets and variables have been injected into your repository by the `azd pipeline config` step
* Create a new file in the `.github\workflows` folder, named `continousdeploy.yml` and place this YAML code into it

```yaml
name: Provision and Deploy

on:
  workflow_dispatch:

jobs:

  build:
    runs-on: ubuntu-latest

    env:
      
      AZURE_CREDENTIALS: ${{ secrets.AZURE_CREDENTIALS }}

    steps:

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: 8.0.102

      - name: Checkout
        uses: actions/checkout@v4

      - name: Install workload
        run: dotnet workload install aspire

      - name: Restore dependencies
        run: dotnet restore

      - name: Build
        run: dotnet build --no-restore

      - name: Install azd
        uses: Azure/setup-azd@v0.1.0
      
      - name: Log in with Azure (Client Credentials)
        if: ${{ env.AZURE_CREDENTIALS != '' }}
        run: |
          $info = $Env:AZURE_CREDENTIALS | ConvertFrom-Json -AsHashtable;
          Write-Host "::add-mask::$($info.clientSecret)"

          azd auth login `
            --client-id "$($info.clientId)" `
            --client-secret "$($info.clientSecret)" `
            --tenant-id "$($info.tenantId)"
        shell: pwsh
        env:
          AZURE_CREDENTIALS: ${{ secrets.AZURE_CREDENTIALS }}

      - name: Log in with Azure (Federated Credentials)
        if: ${{ env.AZURE_CLIENT_ID != '' }}
        run: |
          azd auth login `
            --client-id "$Env:AZURE_CLIENT_ID" `
            --federated-credential-provider "github" `
            --tenant-id "$Env:AZURE_TENANT_ID" --debug
        shell: pwsh

      - name: Provision Infrastructure
        run: azd provision --no-prompt
        env:
          AZURE_ENV_NAME: ${{ vars.AZURE_ENV_NAME }}
          AZURE_LOCATION: ${{ vars.AZURE_LOCATION }}
          AZURE_SUBSCRIPTION_ID: ${{ vars.AZURE_SUBSCRIPTION_ID }}
          AZD_INITIAL_ENVIRONMENT_CONFIG: ${{ secrets.AZD_INITIAL_ENVIRONMENT_CONFIG }}

      - name: Deploy App
        run: azd deploy --no-prompt
        env:
          AZURE_ENV_NAME: ${{ vars.AZURE_ENV_NAME }}
          AZURE_LOCATION: ${{ vars.AZURE_LOCATION }}
          AZURE_SUBSCRIPTION_ID: ${{ vars.AZURE_SUBSCRIPTION_ID }}
          AZD_INITIAL_ENVIRONMENT_CONFIG: ${{ secrets.AZD_INITIAL_ENVIRONMENT_CONFIG }}

```

* Commmit and push your code changes back into your remote GitHub repository
* Go to the `Actions` tab again and run the `Provision and Deploy` action

You'll observe how the GitHub Action logs into Azure, then uses `azd provision` and `azd deploy` to build your app's infrastructure and then deploy your app into it.

---

### Adding messaging

In this phase, you'll add messaging to the app so the frontend can be used to add new items to your todo list. The frontend will be used to collect a new todo item. It will drop a message on an Azure Storage Queue. You'll add a new `BackgroundWorker` class to the backend project that will receive the incoming messages and add them to the server-side todo list. 

* Add the `Aspire.Azure.Storage.Queues` package (version `8.0.0-preview.3.24105.21`) to both the `AppHost`, `ApiService`, and `Web` projects
* Add the `Aspire.Hosting.Azure` package (version `8.0.0-preview.3.24105.21`) to the `AppHost` project
* Update the `AppHost` project's `Program.cs` so that it contains a new service - the Azure Storage reference, along with a second reference to the Queue service Azure Storage offers for asynchronous messaging. 

  ```csharp
  var builder = DistributedApplication.CreateBuilder(args);
  var cache = builder.AddRedis("cache");

  var storage = builder.AddAzureStorage("storage").UseEmulator();

  var queues = storage.AddQueues("queues");

  var apiService = builder.AddProject<Projects.AspireTodo_ApiService>("apiservice")
      .WithReference(queues);

  var frontend = builder.AddProject<Projects.AspireTodo_Web>("webfrontend")
      .WithReference(cache)
      .WithReference(queues)
      .WithReference(apiService);

  builder.Build().Run();
  ```

With Queueing activated app-wide, you can now add support for sending messages into a queue when they're received from the frontend. 

> Note: At this point, when you try to run the `AspireTodo` project, if you lack Docker Desktop, you'll be prompted to install it. That could take about a half-hour, depending on how powerful your machine is (it took about 10 minutes in a new DevBox).

---

### Sending Messages

In this phase, you'll add code to the frontend project that will accept user input and drop it into a queue. 

* In the `Web` project's `Program.cs`, use the `AddAzureQueueService` method to add queueing support to the frontend project

  ```csharp
  // Add service defaults & Aspire components.
  builder.AddServiceDefaults(); // this will be there already

  // Add Storage Queue Support
  builder.AddAzureQueueService("queues");
  builder.AddRedisOutputCache("cache");
  ```

* In the `Web` project's `Componentns\Pages\Todo.razor` file, replace the code you have with this update: 

  ```html
  @page "/"
  @rendermode InteractiveServer
  @using Azure.Storage.Queues
  @inject QueueServiceClient queueServiceClient

  @inject TodoApiClient TodoClient

  <PageTitle>AspireTodo</PageTitle>

  <h1>Todo Items</h1>

  <p>These are the things we AspireTodo.</p>

  @if (todos == null)
  {
      <p><em>Loading...</em></p>
  }
  else
  {
      <table class="table">
          <thead>
              <tr>
                  <th>Description</th>
              </tr>
          </thead>
          <tbody>
              @foreach (var todo in todos.Where(x => !x.IsCompleted))
              {
                  <tr>
                      <td>@todo.Description</td>
                  </tr>
              }
          </tbody>
          <tfoot>
              <tr>
                  <td>
                      <input type="text" @bind="@newTodoItemDescription" />
                      <input type="button" class="btn btn-primary" @onclick="SaveTodo" value="Save" />
                  </td>
              </tr>
          </tfoot>
      </table>
  }

  @code {
      private TodoItem[]? todos;
      private string newTodoItemDescription = "";

      protected override async Task OnInitializedAsync()
      {
          await queueServiceClient.GetQueueClient("incoming").CreateIfNotExistsAsync();
          todos = await TodoClient.GetAllTodoItems();
      }

      private async Task SaveTodo()
      {
          if (!string.IsNullOrEmpty(newTodoItemDescription))
              await queueServiceClient.GetQueueClient("incoming").SendMessageAsync(newTodoItemDescription);
          newTodoItemDescription = "";
      }
  }
  ```

* Run the app and post a few new todo items - you won't see the list update yet, but if you review the logs in the Aspire dashboard you'll see the messages are being sent 

At this point, you're finished with the code required to send messages using an Azure Queue. Commit your code back to the GitHub repository and get ready for the next phase. 

---

### Updating your Azure environment

Now that you've added Azure Storage, when you re-run your Provision & Deploy GitHub Action Workflow again, you'll notice a new resource exists in the resource group that was previously not there. Now you also have an Azure Storage Account. If you go into the Azure portal and go to the Queues section of the Azure Portal for your new Storage Account, you'll see the messages appearing when you hit the "Send" button on the frontend app. 

### Receiving Messages

In this phase, you'll add code to the backend project to start receiving the queued messages, so they can be added to the list of todo items asynchronously. 

* First, add memory cache to the `ApiService` project, and use it to store the list of todo items rather than storing it as a variable in the `Program.cs` by changing the code in the `ApiService` project's `Program.cs` file to contain this code:

  ```csharp
  using Microsoft.Extensions.Caching.Memory;

  var builder = WebApplication.CreateBuilder(args);

  // Add service defaults & Aspire components.
  builder.AddServiceDefaults();

  // Add memory caching to store the todos on the server for now
  builder.Services.AddMemoryCache();

  // Add Azure Storage Queues
  builder.AddAzureQueueService("queues");

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
  ```

* Run the app again to validate that everything is still working as expected, and that your app still shows the 3 todo items it was already showing when the app starts up

* Add a new file to the `ApiService` project named `QueueWorker.cs` and paste this code into it to add a background worker class that watches the Azure Queue and saves incoming queue messages as new todo items. 

  ```csharp
  using Azure.Storage.Queues;
  using Azure.Storage.Queues.Models;
  using Microsoft.Extensions.Caching.Memory;

  public class QueueWorker(QueueServiceClient queueServiceClient, 
      IMemoryCache memoryCache,
      ILogger<QueueWorker> logger) : BackgroundService
  {
      private QueueServiceClient queueServiceClient = queueServiceClient;
      private IMemoryCache memoryCache = memoryCache;
      private readonly ILogger<QueueWorker> logger = logger;

      public override async Task StartAsync(CancellationToken cancellationToken)
      {
          await queueServiceClient.GetQueueClient("incoming").CreateIfNotExistsAsync();
          await base.StartAsync(cancellationToken);
      }

      protected override async Task ExecuteAsync(CancellationToken stoppingToken)
      {
          while (!stoppingToken.IsCancellationRequested)
          {
              var existingTodos = memoryCache.Get<List<TodoItem>>("todos");
              var queue = queueServiceClient.GetQueueClient("incoming");

              QueueMessage[] queuedMessages = await queue.ReceiveMessagesAsync(1, 
                  TimeSpan.FromSeconds(5));
              
              foreach (var message in queuedMessages)
              {
                  if (message.DequeueCount <= 2)
                  {
                      if(existingTodos != null && !existingTodos.Any(x => x.Description.Equals(message.MessageText, 
                          StringComparison.InvariantCultureIgnoreCase)))
                      {  
                          existingTodos.Add(new TodoItem(message.MessageText, false));
                          memoryCache.Set<List<TodoItem>>("todos", existingTodos);
                      }

                      await queue.DeleteMessageAsync(message.MessageId, message.PopReceipt);
                  }
              }

              logger.LogInformation($"Worker running at {DateTime.Now}");

              await Task.Delay(1000);
          }
      }
  }
  ```

* The final step you need to complete to start processing incoming messages is to use the `QueueService` class as a hosted service in the `ApiService` project's `Program.cs`. To do this, add this code after the call to `builder.AddAzureQueueService`:

  ```csharp
  // Add the QueueWorker
  builder.Services.AddHostedService<QueueWorker>();
  ```

Now, you can run the app again and this time, the new todo form should work. Note, the list probably won't refresh as soon as you post a new message; that's because the `QueueWorker` runs once a second to process the incoming messages that are still in the queue. You'd need to add polling or some sort of event-based mechanism (like even another queue!) to update the user interface when the list changes. We won't do that in this class (feel free to do so if you have the time), but it is one of the considerations developers using asynchronous messaging need to make when building these kinds of distributed applications. 

If you re-deploy the app now using the Provision & Deploy CI/CD action after committing your code, you'll see all of the new functionality light up. 

---

### Storing data in a Postgres database

In this final phase of the exercises, you'll add a persistent database to the equation so your todo data persists even when the app restarts.

* Add a new Class Library project named `AspireTodo.TodoDatabase` to the solution
* Add a new file named `Todo.cs` to the `TodoDatabase` project. Paste this code into that file for the entity definition:

   ```csharp
  public class Todo
  {
      public int Id { get; set; }
      public string Description { get; set; } = string.Empty;
      public bool IsCompleted { get; set; }
  } 
   ```
* Add a reference in the `TodoDatabase` project to the Aspire component `Aspire.Npgsql.EntityFrameworkCore.PostgreSQL` (version `8.0.0-preview.3.24105.21`). This will provide all of the data access services for your PostgreSQL database.
* Create a new file named `TodoDatabaseDbContext.cs` in the `TodoDatabase` project and paste the following. Think of the `DbContext` as an interface for the API to manipulate your database:

   ```csharp
   using Microsoft.EntityFrameworkCore;
   using Microsoft.EntityFrameworkCore.Metadata.Builders;

  public class TodoDatabaseDbContext(DbContextOptions<TodoDatabaseDbContext> options) : DbContext(options)
  {
      public DbSet<Todo> TodoItems => Set<Todo>();

      protected override void OnModelCreating(ModelBuilder builder)
      {
          DefineTodoType(builder.Entity<Todo>());
      }

      private static void DefineTodoType(EntityTypeBuilder<Todo> builder)
      {
          builder.ToTable("todo");

          builder.HasKey(ci => ci.Id);

          builder.Property(ci => ci.Id)
              .UseHiLo("todo_type_hilo")
              .IsRequired();

          builder.Property(cb => cb.Description)
              .IsRequired()
              .HasMaxLength(128);
      }
  }
  ```

* Add a new Web API project, enlisting in Aspire orchestration (and uncheck controller usage so you get Minimal APIs) named `AspireTodo.TodoDatabaseManager`
* Like with the `ApiService` project, remove all the "Weather" related code from the `Program.cs` when the project is added 
* Add a reference to the Aspire component `Aspire.Npgsql.EntityFrameworkCore.PostgreSQL` (version `8.0.0-preview.3.24105.21`). This will provide all of the data access services for your PostgreSQL database.
* Add a reference to the NuGet package `Microsoft.EntityFrameworkCore.Design` (version `8.0.1`). This enables migrations, EF Core's mechanism for tracking and deploying database changes.
* Install the .NET EF tool by entering this command at your terminal:

  ```text
  dotnet tool install --global dotnet-ef --version 8.0.1
  ```

* In the `Program.cs` file for your API service, make the necessary changes to move from an in-memory cache to your database. First, remove the namespace and middleware configuration for the in-memory cache.
   - Remove the using for `Microsoft.Extensions.Cache.Memory`
   - Remove the line `builder.Services.AddMemoryCache()`
   - Remove the command to seed the memory cache. This is multiple lines starting with `app.Services.GetRequiredService<IMemoryCache>`.

* Add a reference from the `ApiService` project to the `TodoDatabase` project
* Wire in the database. After the `QueueWorker` is configured as a hosted service, inform DI about your database:

   ```csharp
   builder.AddNpgsqlDbContext<TodoDatabaseDbContext>("tododatabase");
   ```

* Update the `/todos` endpoint to use the database instead of the memory cahce:

   ```csharp
   app.MapGet("/todos", (TodoDatabaseDbContext ctx) => ctx.TodoItems.ToArray());

* The asynchronous messaging service needs to be updated to process database records rather than using the in-memory cache. Replace the code in `QueueWorker.cs` to look like this instead:

   ```csharp
   using Azure.Storage.Queues;
   using Azure.Storage.Queues.Models;

   public class QueueWorker(QueueServiceClient queueServiceClient,
      IServiceProvider serviceProvider,
      ILogger<QueueWorker> logger) : BackgroundService
  {
      private QueueServiceClient queueServiceClient = queueServiceClient;
      private readonly IServiceProvider serviceProvider = serviceProvider;
      private readonly ILogger<QueueWorker> logger = logger;

      public override async Task StartAsync(CancellationToken cancellationToken)
      {
          await queueServiceClient.GetQueueClient("incoming").CreateIfNotExistsAsync();
          await base.StartAsync(cancellationToken);
      }

      protected override async Task ExecuteAsync(CancellationToken stoppingToken)
      {
          using var scope = serviceProvider.CreateScope();
          var todoDatabaseDbContext = scope.ServiceProvider.GetRequiredService<TodoDatabaseDbContext>();

          while (!stoppingToken.IsCancellationRequested)
          {
              try
              {
                  // database might not be up yet
                  var existingTodos = todoDatabaseDbContext.TodoItems.ToList();
                  var queue = queueServiceClient.GetQueueClient("incoming");

                  QueueMessage[] queuedMessages = await queue.ReceiveMessagesAsync(1,
                      TimeSpan.FromSeconds(5));

                  foreach (var message in queuedMessages)
                  {
                      if (message.DequeueCount <= 2)
                      {
                          if (existingTodos != null && !existingTodos.Any(x => x.Description.Equals(message.MessageText,
                              StringComparison.InvariantCultureIgnoreCase)))
                          {
                              todoDatabaseDbContext.TodoItems.Add(new Todo { Description = message.MessageText, IsCompleted = false });
                          }

                          await queue.DeleteMessageAsync(message.MessageId, message.PopReceipt);
                      }
                  }
                  await todoDatabaseDbContext.SaveChangesAsync();
              } 
              catch(Exception ex)
              {
                  logger.LogError(ex, "Error during startup");
              }
              
              logger.LogInformation($"Worker running at {DateTime.Now}");

              await Task.Delay(1000);
          }
      }
  }
   ```

* Let's prepare the database for local testing and deployment. The first step is to create a snapshot of the database, called a "migration", for EF Core to use when creating the database or updating it to match a change to the schema. Create a new ASP.NET Core API web project and name it, `TodoDatabaseManager`. Include a refeence to the `TodoDatabase` project.

* From the root of the `TodoDatabaseManager` project, run this command. It will take a snapshot of the database and create the code to define it, called a "migration."

   ```text
   dotnet ef migrations add InitialCreate
   ```

* Set up your database and populate the connection string in your app configuration. Under the connection strings section, add it like this:

   ```json
   "ConnectionStrings": {
 
     // A connection string is here to enable use of the `dotnet ef` cmd line tool from the project root.
     // If the configuration value is not present or not well-formed, the app will fail at startup.
     // Note that some commands require the connection string to point to a real database in order to fully
     // function (e.g. `dotnet ef database update`, `dotnet ef migrations list`).
     "tododatabase": "Server=localhost;Port=5432;Database=NOT_A_REAL_DB"
   ```

* From the root of the `TodoDatabaseManager` project, run this command. It will take a snapshot of the database and create the code to define it, called a "migration."

   ```text
   dotnet ef migrations add InitialCreate
   ```

* Use the migration to create or update your database by running the command: 

   ```text
   dotnet ef database update
   ```

* Create the database initializer class as `DatabaseInitializer.cs`. This will run as a background service and create/seed the database when neccessary.

   ```csharp
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
   ```

* Update `Program.cs` by including this code after the c all to `AddServiceDefaults`. This code registers the database and informs EF Core where to find the migrations that define it. It then adds telemetry and configures a call to the initializer through a background service.

   ```csharp
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
   ```

Rock and roll!

