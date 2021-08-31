namespace ClickView.GoodStuff.AspNetCore.Authentication.TokenValidation
{
    using System;
    using Microsoft.Extensions.Options;

    internal sealed class TokenValidatorOptionsConfigureOptions : IPostConfigureOptions<TokenValidatorOptions>
    {
        private readonly OpenIdConnectSessionHandlerOptions _handlerOptions;

        public TokenValidatorOptionsConfigureOptions(IOptions<OpenIdConnectSessionHandlerOptions> handlerOptions)
        {
            _handlerOptions = handlerOptions.Value;
        }

        public void PostConfigure(string name, TokenValidatorOptions options)
        {
            options = options ?? throw new ArgumentNullException(nameof(options));

            options.Authority ??= _handlerOptions.Authority;
            options.DefaultAudience ??= _handlerOptions.DefaultAudience;
        }
    }
}
