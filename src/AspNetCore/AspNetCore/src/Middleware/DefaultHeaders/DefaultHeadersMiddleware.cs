namespace ClickView.GoodStuff.AspNetCore.Middleware.DefaultHeaders
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// Middleware to add a set of default headers specified in <see cref="DefaultHeadersMiddlewareOptions"/> to every response
    /// </summary>
    public class DefaultHeadersMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly DefaultHeadersMiddlewareOptions _options;

        /// <summary>
        /// Creates a new <see cref="DefaultHeadersMiddleware"/>
        /// </summary>
        /// <param name="next"></param>
        /// <param name="options"></param>
        public DefaultHeadersMiddleware(RequestDelegate next, IOptions<DefaultHeadersMiddlewareOptions> options)
        {
            _next = next;
            _options = options.Value;
        }

        /// <summary>
        /// Invokes the logic of the middleware
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task Invoke(HttpContext context)
        {
            if (_options.DefaultHeaders.Count == 0)
                return _next(context);

            context.Response.OnStarting(() =>
            {
                foreach (var (key, value) in _options.DefaultHeaders)
                {
                    context.Response.Headers.TryAdd(key, value);
                }

                return Task.CompletedTask;
            });

            return _next(context);
        }
    }
}
