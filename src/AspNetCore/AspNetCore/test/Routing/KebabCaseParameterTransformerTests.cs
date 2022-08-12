namespace ClickView.GoodStuff.AspNetCore.Tests.Routing;

using AspNetCore.Routing;
using Xunit;

public class KebabCaseParameterTransformerTests
{
    [Theory]
    [InlineData("Hello", "Hello")]
    [InlineData("Hello-World", "HelloWorld")]
    [InlineData("Hello-Hello-Hello", "HelloHelloHello")]
    public void TransformOutbound(string expectedValue, object? inputValue)
    {
        var transformer = new KebabCaseParameterTransformer();

        Assert.Equal(expectedValue, transformer.TransformOutbound(inputValue));
    }

    [Fact]
    public void TransformOutbound_Null_ReturnsNull()
    {
        var transformer = new KebabCaseParameterTransformer();

        Assert.Null(transformer.TransformOutbound(null));
    }
}
