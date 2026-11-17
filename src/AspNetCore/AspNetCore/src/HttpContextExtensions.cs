namespace ClickView.GoodStuff.AspNetCore;

using Microsoft.AspNetCore.Http;

public static class HttpContextExtensions
{
    public static bool IsAjaxEndpoint(this HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var endpoint = context.GetEndpoint();
        var ajaxAttribute = endpoint?.Metadata.GetMetadata<AjaxControllerAttribute>();

        return ajaxAttribute is not null;
    }
}
