
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
