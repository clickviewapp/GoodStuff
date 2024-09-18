namespace ClickView.GoodStuff.AspNetCore.VersionPage;

using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseVersionPage(this IApplicationBuilder app,
        PathString path, ApplicationInformation information)
    {
        ArgumentNullException.ThrowIfNull(app);
        ArgumentNullException.ThrowIfNull(information);

        UseVersionPageCore(app, path, [information]);

        return app;
    }

    public static IApplicationBuilder UseVersionPage(this IApplicationBuilder app,
        PathString path, ApplicationInformation information, VersionPageOptions options)
    {
        ArgumentNullException.ThrowIfNull(app);
        ArgumentNullException.ThrowIfNull(information);
        ArgumentNullException.ThrowIfNull(options);

        UseVersionPageCore(app, path, [information, Options.Create(options)]);

        return app;
    }

    private static void UseVersionPageCore(this IApplicationBuilder app, PathString path, object[] args)
    {
        app.MapWhen(Predicate, b => b.UseMiddleware<VersionPageMiddleware>(args));

        return;

        bool Predicate(HttpContext c) => c.Request.Path.StartsWithSegments(path, out var remaining) &&
                                         string.IsNullOrEmpty(remaining);
    }
}
