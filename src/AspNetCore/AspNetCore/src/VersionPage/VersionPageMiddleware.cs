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

        public VersionPageMiddleware(RequestDelegate next, ApplicationInformation applicationInformation,
            IOptions<VersionPageOptions> options)
        {
            ArgumentNullException.ThrowIfNull(next);
            ArgumentNullException.ThrowIfNull(applicationInformation);
            ArgumentNullException.ThrowIfNull(options);

            _next = next;
            _applicationInformation = applicationInformation;
            _options = options.Value;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            ArgumentNullException.ThrowIfNull(httpContext);

            httpContext.Response.StatusCode = 200;

            if (_options.ResponseWriter != null)
                await _options.ResponseWriter(httpContext, _applicationInformation);
        }
    }
}
