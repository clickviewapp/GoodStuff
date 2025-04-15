// See https://aka.ms/new-console-template for more information

using ClickView.GoodStuff.Queues.RabbitMq;
using ClickView.GoodStuff.Queues.RabbitMq.Serialization;
using Microsoft.Extensions.Logging;

var loggerFactory = LoggerFactory.Create(b => b.AddConsole());

var queueClient = new RabbitMqClient(new RabbitMqClientOptions
{
    Host = "localhost",
    Port = 5672,
    Username = "guest",
    Password = "guest",
    Serializer = new SystemTextJsonMessageSerializer(),
    LoggerFactory = loggerFactory
});

var subscribeLog = loggerFactory.CreateLogger("Subscribe");

await queueClient.SubscribeAsync<TestData>("test-queue", async (context, token) =>
{
    subscribeLog.LogDebug("{@Context}", context);
    subscribeLog.LogInformation("Message: {Message}", context.Data.Message);

    if (context.Priority.IsHighPriority())
        subscribeLog.LogInformation("High priority message");

    if (context.DeliveryCount is null)
    {
        subscribeLog.LogInformation("No Delivery Count");
    }
    else
    {
        subscribeLog.LogInformation("DeliveryCount: {DeliveryCount}", context.DeliveryCount);

        if (context.DeliveryCount > context.Data.MaxDeliver)
        {
            subscribeLog.LogInformation("MaxDeliver reached. Acking");
            await context.AcknowledgeAsync(cancellationToken: token);
            return;
        }
    }

    if (context.Data.ShouldAck)
    {
        subscribeLog.LogInformation("ShouldAck. Acking");
        await context.AcknowledgeAsync(cancellationToken: token);
    }
    else
    {
        subscribeLog.LogInformation("ShouldAck false. Nacking");
        await context.NegativeAcknowledgeAsync(cancellationToken: token);
    }
});

var logger = loggerFactory.CreateLogger("Main");

Console.WriteLine("Hit enter to start!");
Console.ReadLine();
await EnqueueAsync(new TestData {Message = "Basic message. Should ack"});

await EnqueueAsync(new TestData
{
    Message = "Basic message, Should Ack false. Max deliver 5",
    ShouldAck = false
});

await EnqueueAsync(new TestData {Message = "High priority message"},
    new EnqueueOptions {Priority = MessagePriority.High});

return;

async Task EnqueueAsync(TestData testData, EnqueueOptions? enqueueOptions = null)
{
    logger.LogInformation("Enqueuing {Message}", testData.Message);
    await queueClient.EnqueueAsync("test-exchange", testData, enqueueOptions);
    logger.LogInformation("waiting...");
    await Task.Delay(1000);
}

internal class TestData
{
    public required string Message { get; set; }
    public bool ShouldAck { get; set; } = true;
    public int MaxDeliver { get; set; } = 5;
}
