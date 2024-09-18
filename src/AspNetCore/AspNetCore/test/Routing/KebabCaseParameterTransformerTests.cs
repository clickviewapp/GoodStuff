namespace ClickView.GoodStuff.AspNetCore.Tests.Routing;

using AspNetCore.Routing;
using Xunit;

public class KebabCaseParameterTransformerTests
{
    [Theory]
    [InlineData("hello", "Hello")]
    [InlineData("hello-world", "HelloWorld")]
    [InlineData("hello-hello-hello", "HelloHelloHello")]
    [InlineData("lowercasewords", "lowercasewords")]
    public void TransformOutbound_Lowercase(string expectedValue, object? inputValue)
    {
        var transformer = new KebabCaseParameterTransformer(true);

        Assert.Equal(expectedValue, transformer.TransformOutbound(inputValue));
    }

    [Theory]
    [InlineData("Hello", "Hello")]
    [InlineData("Hello-World", "HelloWorld")]
    [InlineData("Hello-Hello-Hello", "HelloHelloHello")]
    [InlineData("lowercasewords", "lowercasewords")]
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
