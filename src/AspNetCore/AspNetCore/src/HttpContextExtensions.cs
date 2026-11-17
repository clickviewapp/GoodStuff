namespace ClickView.GoodStuff.AspNetCore;

using Microsoft.AspNetCore.Http;

public static class HttpContextExtensions
{
    /// <summary>
    /// Returns true if the current endpoint is marked with <see cref="AjaxControllerAttribute"/>.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    public static bool IsAjaxEndpoint(this HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var endpoint = context.GetEndpoint();
        var ajaxAttribute = endpoint?.Metadata.GetMetadata<AjaxControllerAttribute>();

        return ajaxAttribute is not null;
    }
}
