namespace ClickView.GoodStuff.Queues.RabbitMq.Internal;

using RabbitMQ.Client;

internal static class BasicPropertiesExtensions
{
    internal static bool TryGetHeaderValue<T>(this IReadOnlyBasicProperties properties, string headerName, out T? value)
    {
        if (properties.Headers is null)
        {
            value = default;
            return false;
        }

        if (!properties.Headers.TryGetValue(headerName, out var headerValue))
        {
            value = default;
            return false;
        }

        if (headerValue is null)
        {
            value = default;
            return false;
        }

        if (headerValue is not T typedValue)
        {
            throw new InvalidCastException(
                $"Unable to cast RabbitMq header '{headerName}' from type '{headerValue.GetType().FullName}' to type '{typeof(T).FullName}'.");
        }

        value = typedValue;
        return true;
    }
}
