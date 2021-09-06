namespace ClickView.GoodStuff.AspNetCore.VersionPage
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Options;

    public sealed class VersionPageMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ApplicationInformation _applicationInformation;
        private readonly VersionPageOptions _options;

        public VersionPageMiddleware(
            RequestDelegate next, ApplicationInformation applicationInformation, IOptions<VersionPageOptions> options)
        {
            if (next == null)
                throw new ArgumentNullException(nameof(next));

            if (applicationInformation == null)
                throw new ArgumentNullException(nameof(applicationInformation));

            if (options == null)
                throw new ArgumentNullException(nameof(options));

            _next = next;
            _applicationInformation = applicationInformation;
            _options = options.Value;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            if (httpContext == null)
                throw new ArgumentNullException(nameof(httpContext));

            httpContext.Response.StatusCode = 200;

            if (_options.ResponseWriter != null)
                await _options.ResponseWriter(httpContext, _applicationInformation);
        }
    }
}
