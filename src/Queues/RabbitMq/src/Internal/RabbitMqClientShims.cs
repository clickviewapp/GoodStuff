namespace ClickView.GoodStuff.Queues.RabbitMq.Internal;

using System.Reflection;
using RabbitMQ.Client;

internal static class RabbitMqClientShims
{
    public static Task CloseAsync(this IModel model)
    {
        var eventArgs = new ShutdownEventArgs(ShutdownInitiator.Application, Constants.ReplySuccess,
            "GoodBye");

        return model.CloseAsync(eventArgs, false);
    }

    public static Task CloseAsync(this IModel model, ShutdownEventArgs reason, bool abort)
    {
        // Get the `_delegate` field
        var delegateField = model.GetType().GetField("_delegate", BindingFlags.NonPublic | BindingFlags.Instance);

        // Get the value of that field
        var recoveryAwareModel = delegateField!.GetValue(model);

        // Get the close method with ShutdownEventArgs and bool as the args
        var closeMethod = recoveryAwareModel!.GetType().GetMethod("Close", new[] { typeof(ShutdownEventArgs), typeof(bool) });

        // Call that method
        return (Task) closeMethod!.Invoke(recoveryAwareModel, new object[] { reason, abort })!;
    }
}
