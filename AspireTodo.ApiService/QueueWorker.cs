
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
