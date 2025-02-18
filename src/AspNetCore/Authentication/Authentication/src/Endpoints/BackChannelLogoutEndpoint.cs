namespace ClickView.GoodStuff.AspNetCore.Authentication.Endpoints;

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

internal sealed class BackChannelLogoutEndpoint(
    ITokenValidator tokenValidator,
    IUserSessionStore sessionStore,
    ILogger<BackChannelLogoutEndpoint> logger)
    : IEndpoint
{
    public async Task ProcessAsync(HttpContext context, CancellationToken token = default)
    {
        logger.LogDebug("Processing Back-Channel logout");

        context.Response.Headers.CacheControl = "no-cache, no-store";
        context.Response.Headers.Pragma = "no-cache";

        if (!context.Request.HasFormContentType)
        {
            logger.LogWarning("Failed to process Back-Channel logout");

            context.Response.StatusCode = 400;
            return;
        }

        try
        {
            var logoutToken = context.Request.Form[OidcConstants.BackChannelLogoutRequest.LogoutToken]
                .FirstOrDefault();

            if (string.IsNullOrWhiteSpace(logoutToken))
            {
                logger.LogWarning("Failed to process Back-Channel logout. Missing logout token");

                context.Response.StatusCode = 400;
                return;
            }

            var user = await tokenValidator.ValidateLogoutTokenAsync(logoutToken);

            var sessionId = user.FindFirst(JwtClaimTypes.SessionId)?.Value;
            if (string.IsNullOrWhiteSpace(sessionId))
            {
                logger.LogWarning("Backchannel logout does not contain a session id");

                context.Response.StatusCode = 400;
                return;
            }

            await sessionStore.DeleteBySessionIdAsync(sessionId, token);

            logger.LogInformation("Back-Channel logout successful for SessionId: {Sid}", sessionId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to process Back-Channel logout");
            context.Response.StatusCode = 400;
        }
    }
}
