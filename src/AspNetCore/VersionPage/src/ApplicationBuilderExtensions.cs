namespace ClickView.GoodStuff.AspNetCore.VersionPage
{
    using System;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Options;

    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseVersionPage(this IApplicationBuilder app,
            PathString path, ApplicationInformation information)
        {
            if (app == null)
                throw new ArgumentNullException(nameof(app));

            if (information == null)
                throw new ArgumentNullException(nameof(information));

            UseVersionPageCore(app, path, new object[] {information});

            return app;
        }

        public static IApplicationBuilder UseVersionPage(this IApplicationBuilder app,
            PathString path, ApplicationInformation information, VersionPageOptions options)
        {
            if (app == null)
                throw new ArgumentNullException(nameof(app));

            if (information == null)
                throw new ArgumentNullException(nameof(information));

            if (options == null)
                throw new ArgumentNullException(nameof(options));

            UseVersionPageCore(app, path, new object[] {information, Options.Create(options)});

            return app;
        }

        private static void UseVersionPageCore(this IApplicationBuilder app,
            PathString path, object[] args)
        {
            Func<HttpContext, bool> predicate = c =>
                c.Request.Path.StartsWithSegments(path, out var remaining) &&
                string.IsNullOrEmpty(remaining);

            app.MapWhen(predicate, b => b.UseMiddleware<VersionPageMiddleware>(args));
        }
    }
}
