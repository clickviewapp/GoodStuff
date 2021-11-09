namespace ClickView.GoodStuff.AspNetCore.Middleware.DefaultHeaders
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Extensions.Primitives;

    /// <summary>
    /// Options for <see cref="DefaultHeadersMiddleware"/>
    /// </summary>
    public class DefaultHeadersMiddlewareOptions
    {
        private IDictionary<string, StringValues> _defaultHeaders = new Dictionary<string, StringValues>();

        /// <summary>
        /// The collection of default headers
        /// </summary>
        public IDictionary<string, StringValues> DefaultHeaders
        {
            get => _defaultHeaders;
            set => _defaultHeaders = value ?? throw new ArgumentNullException(nameof(value));
        }
    }
}