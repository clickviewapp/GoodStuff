namespace ClickView.GoodStuff.AspNetCore.Authorization
{
    using System;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Extension methods for <see cref="IServiceCollection"/>
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register services to support ajax requests
        /// </summary>
        /// <param name="services"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void AddAjaxServices(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            // Register the 401/403 handler
            services.AddTransient<IAuthorizationMiddlewareResultHandler, AjaxAuthorizationMiddlewareResultHandler>();
        }
    }
}
