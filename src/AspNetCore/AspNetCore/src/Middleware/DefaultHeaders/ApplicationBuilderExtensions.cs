﻿namespace ClickView.GoodStuff.AspNetCore.Middleware.DefaultHeaders
{
    using System;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// Extension methods for adding default headers to the application
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Adds middleware that adds default headers to the responses
        /// </summary>
        /// <param name="app"></param>
        /// <param name="options"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void UseDefaultHeaders(this IApplicationBuilder app,
            DefaultHeadersMiddlewareOptions options)
        {
            ArgumentNullException.ThrowIfNull(app);
            ArgumentNullException.ThrowIfNull(options);

            app.UseMiddleware<DefaultHeadersMiddleware>(Options.Create(options));
        }
    }
}
