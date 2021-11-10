namespace ClickView.GoodStuff.AspNetCore.Authorization
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Authorization.Policy;
    using Microsoft.AspNetCore.Http;

    /// <summary>
    /// A <see cref="IAuthorizationMiddlewareResultHandler"/> handler which will make controllers with the
    /// <see cref="AjaxControllerAttribute"/> attribute return 401 or 403 status codes for challenged or forbidden requests
    /// </summary>
    public class AjaxAuthorizationMiddlewareResultHandler : IAuthorizationMiddlewareResultHandler
    {
        private readonly AuthorizationMiddlewareResultHandler _handler;

        /// <summary>
        /// Create a new instance of <see cref="AjaxAuthorizationMiddlewareResultHandler"/>
        /// </summary>
        public AjaxAuthorizationMiddlewareResultHandler()
        {
            _handler = new AuthorizationMiddlewareResultHandler();
        }

        /// <inheritdoc />
        public Task HandleAsync(RequestDelegate next, HttpContext context, AuthorizationPolicy policy,
            PolicyAuthorizationResult authorizeResult)
        {
            var endpoint = context.GetEndpoint();

            // If we have no endpoint, fall back to the default handler
            if (endpoint == null)
                return _handler.HandleAsync(next, context, policy, authorizeResult);

            var ajaxAttribute = endpoint.Metadata.GetMetadata<AjaxControllerAttribute>();

            // If we have no attribute, fall back to the default handler
            if (ajaxAttribute == null)
                return _handler.HandleAsync(next, context, policy, authorizeResult);

            if (authorizeResult.Challenged)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.CompletedTask;
            }

            if (authorizeResult.Forbidden)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                return Task.CompletedTask;
            }

            // Fall back to the default handler
            return _handler.HandleAsync(next, context, policy, authorizeResult);
        }
    }
}
