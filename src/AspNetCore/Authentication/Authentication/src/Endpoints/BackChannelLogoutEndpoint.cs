namespace ClickView.GoodStuff.AspNetCore.Authentication.Endpoints
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Abstractions;
    using IdentityModel;
    using Infrastructure;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using TokenValidation;

    internal sealed class BackChannelLogoutEndpoint : IEndpoint
    {
        private readonly ITokenValidator _tokenValidator;
        private readonly IUserSessionStore _sessionStore;
        private readonly ILogger<BackChannelLogoutEndpoint> _logger;

        public BackChannelLogoutEndpoint(ITokenValidator tokenValidator,
            IUserSessionStore sessionStore,
            ILogger<BackChannelLogoutEndpoint> logger)
        {
            _tokenValidator = tokenValidator;
            _sessionStore = sessionStore;
            _logger = logger;
        }

        public async Task ProcessAsync(HttpContext context, CancellationToken token = default)
        {
            _logger.LogDebug("Processing Back-Channel logout");

            context.Response.Headers.Add("Cache-Control", "no-cache, no-store");
            context.Response.Headers.Add("Pragma", "no-cache");

            if (!context.Request.HasFormContentType)
            {
                _logger.LogWarning("Failed to process Back-Channel logout");

                context.Response.StatusCode = 400;
                return;
            }

            try
            {
                var logoutToken = context.Request.Form[OidcConstants.BackChannelLogoutRequest.LogoutToken]
                    .FirstOrDefault();

                if (string.IsNullOrWhiteSpace(logoutToken))
                {
                    _logger.LogWarning("Failed to process Back-Channel logout. Missing logout token");

                    context.Response.StatusCode = 400;
                    return;
                }

                var user = await _tokenValidator.ValidateLogoutTokenAsync(logoutToken);

                var sessionId = user.FindFirst(JwtClaimTypes.SessionId)?.Value;
                if (string.IsNullOrWhiteSpace(sessionId))
                {
                    _logger.LogWarning("Backchannel logout does not contain a session id");

                    context.Response.StatusCode = 400;
                    return;
                }

                await _sessionStore.DeleteBySessionIdAsync(sessionId, token);

                _logger.LogInformation("Back-Channel logout successful for SessionId: {Sid}", sessionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process Back-Channel logout");
                context.Response.StatusCode = 400;
            }
        }
    }
}
