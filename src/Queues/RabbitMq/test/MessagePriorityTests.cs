namespace ClickView.GoodStuff.Queues.RabbitMq.Tests;

using Xunit;

public class MessagePriorityTests
{
    [Fact]
    public void High_Equals5()
    {
        var priority = MessagePriority.High;

        Assert.Equal(5, priority.Value);
        Assert.Equal(5, priority);
    }

    [Fact]
    public void Normal_Equals0()
    {
        var priority = MessagePriority.Normal;

        Assert.Equal(0, priority.Value);
        Assert.Equal(0, priority);
    }

    [Fact]
    public void IsHighPriority_High_ReturnsTrue()
    {
        var priority = MessagePriority.High;

        Assert.True(priority.IsHighPriority());
    }

    [Theory]
    [InlineData(0, true)]
    [InlineData(4, true)]
    [InlineData(5, false)]
    public void IsNormalPriority(byte priority, bool expected)
    {
        var messagePriority = new MessagePriority(priority);
        Assert.Equal(expected, messagePriority.IsNormalPriority());
    }

    [Fact]
    public void Equals_SameValue_ReturnsTrue()
    {
        var p1 = new MessagePriority(5);
        var p2 = MessagePriority.High;

        Assert.Equal(p1, p2);
    }

    [Fact]
    public void Equals_DifferentValue_ReturnsFalse()
    {
        var p1 = new MessagePriority(2);
        var p2 = new MessagePriority(5);

        Assert.NotEqual(p1, p2);
    }

    [Fact]
    public void EqualsOperator_SameValue_ReturnsTrue()
    {
        var p1 = new MessagePriority(5);
        var p2 = MessagePriority.High;

        Assert.True(p1 == p2);
    }

    [Fact]
    public void NotEqualOperator_DifferentValue_ReturnsTrue()
    {
        var p1 = new MessagePriority(2);
        var p2 = new MessagePriority(5);

        Assert.True(p1 != p2);
    }
}
