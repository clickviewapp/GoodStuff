namespace ClickView.GoodStuff.AspNetCore.Authentication
{
    using System;
    using Abstractions;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Builder class for the OpenIdSession handling
    /// </summary>
    public class OpenIdConnectSessionHandlerBuilder
    {
        /// <summary>
        /// Provides access to the <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" /> passed to this object's constructor.
        /// </summary>
        public IServiceCollection Services { get; }

        /// <summary>
        /// Creates a new instance of <see cref="OpenIdConnectSessionHandlerBuilder"/>
        /// </summary>
        /// <param name="services"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public OpenIdConnectSessionHandlerBuilder(IServiceCollection services)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
        }

        /// <summary>
        /// Add an in-memory <see cref="IUserSessionStore"/>
        /// </summary>
        /// <returns></returns>
        public OpenIdConnectSessionHandlerBuilder AddInMemoryUserSessionStore()
        {
            Services.AddSingleton<IUserSessionStore, InMemoryUserSessionStore>();
            return this;
        }
    }
}
