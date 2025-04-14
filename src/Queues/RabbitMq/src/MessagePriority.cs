namespace ClickView.GoodStuff.Queues.RabbitMq;

using System.Diagnostics.CodeAnalysis;

public readonly struct MessagePriority(byte value) : IEquatable<MessagePriority>
{
    public byte Value { get; } = value;

    /// <summary>
    /// Returns true if the message priority is between 0 and 4.
    /// </summary>
    /// <returns></returns>
    public bool IsNormalPriority() => Value < 5;

    /// <summary>
    /// Returns true if the message priority is higher than 4.
    /// </summary>
    /// <returns></returns>
    public bool IsHighPriority() => Value > 4;

    public static readonly MessagePriority High = new(5);
    public static readonly MessagePriority Normal = new(0);

    public static implicit operator byte(MessagePriority priority) => priority.Value;
    public static implicit operator MessagePriority(byte value) => new(value);

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return base.Equals(obj);
    }

    public bool Equals(MessagePriority other)
    {
        return Value == other.Value;
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public static bool operator ==(MessagePriority left, MessagePriority right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(MessagePriority left, MessagePriority right)
    {
        return !left.Equals(right);
    }
}
