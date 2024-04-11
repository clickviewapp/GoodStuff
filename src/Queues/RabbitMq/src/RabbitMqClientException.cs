namespace ClickView.GoodStuff.Queues.RabbitMq;

public class RabbitMqClientException : Exception
{
    public RabbitMqClientException(string message) : base(message)
    {
    }
}
