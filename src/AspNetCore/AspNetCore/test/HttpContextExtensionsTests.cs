namespace ClickView.GoodStuff.AspNetCore.Tests;

using Microsoft.AspNetCore.Http;
using Xunit;

public class HttpContextExtensionsTests
{
    // Endpoints marked with the ajax attribute are detected.
    [Fact]
    public void IsAjaxEndpoint_EndpointWithAjaxAttribute_ReturnsTrue()
    {
        var metadata = new EndpointMetadataCollection(new AjaxControllerAttribute());
        var endpoint = new Endpoint(requestDelegate: null, metadata, displayName: null);
        var context = CreateContext(endpoint);

        Assert.True(context.IsAjaxEndpoint());
    }

    // Endpoints without the ajax attribute are not treated as ajax.
    [Fact]
    public void IsAjaxEndpoint_EndpointWithoutAjaxAttribute_ReturnsFalse()
    {
        var endpoint = new Endpoint(requestDelegate: null, EndpointMetadataCollection.Empty, displayName: null);
        var context = CreateContext(endpoint);

        Assert.False(context.IsAjaxEndpoint());
    }

    // Requests that have not been routed to an endpoint are not treated as ajax.
    [Fact]
    public void IsAjaxEndpoint_NoEndpoint_ReturnsFalse()
    {
        var context = CreateContext(null);

        Assert.False(context.IsAjaxEndpoint());
    }

    private static DefaultHttpContext CreateContext(Endpoint? endpoint)
    {
        var context = new DefaultHttpContext();
        context.SetEndpoint(endpoint);

        return context;
    }
}
