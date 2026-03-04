namespace ClickView.GoodStuff.Queues.RabbitMq;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

internal class PostRabbitMqOptions(IServiceProvider serviceProvider) : IPostConfigureOptions<RabbitMqClientOptions>
{
    public void PostConfigure(string? name, RabbitMqClientOptions options)
    {
        if (options.LoggerFactory is null)
        {
            if (serviceProvider.GetService(typeof(ILoggerFactory)) is ILoggerFactory loggerFactory)
                options.LoggerFactory = loggerFactory;
        }
    }
}
