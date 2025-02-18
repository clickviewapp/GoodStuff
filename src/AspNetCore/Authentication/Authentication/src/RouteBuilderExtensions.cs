namespace ClickView.GoodStuff.AspNetCore.Authentication;

using System.Threading.Tasks;
using Endpoints;
using Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

public static class RouteBuilderExtensions
{
    public static void UseBackChannelEndpoint(this IEndpointRouteBuilder builder, PathString pattern)
    {
        builder.MapPost(pattern, UseEndpoint<BackChannelLogoutEndpoint>);
    }

    private static Task UseEndpoint<T>(HttpContext context) where T : IEndpoint
    {
        var service = context.RequestServices.GetRequiredService<T>();
        return service.ProcessAsync(context, context.RequestAborted);
    }
}
