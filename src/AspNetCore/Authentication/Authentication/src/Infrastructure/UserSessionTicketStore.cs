namespace ClickView.GoodStuff.AspNetCore.Authentication.Infrastructure;

using System;
using System.Threading.Tasks;
using Abstractions;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;

internal sealed class UserSessionTicketStore(
    IUserSessionStore userSessionStore,
    IDataSerializer<AuthenticationTicket> ticketSerializer,
    ILogger<UserSessionTicketStore> logger)
    : IUserSessionTicketStore
{
    public async Task<string> StoreAsync(AuthenticationTicket ticket)
    {
        var key = CryptoRandom.CreateUniqueId(format: CryptoRandom.OutputFormat.Hex);

        var userSession = CreateUserSession(key, ticket);

        // ensure that we don't re-add the same session
        if (userSession.SessionId != null)
            await userSessionStore.DeleteBySessionIdAsync(userSession.SessionId);

        await userSessionStore.AddAsync(userSession);

        return key;
    }

    public Task RenewAsync(string key, AuthenticationTicket ticket)
    {
        var userSession = CreateUserSession(key, ticket);

        return userSessionStore.UpdateAsync(key, userSession);
    }

    public async Task<AuthenticationTicket?> RetrieveAsync(string key)
    {
        var userSession = await userSessionStore.GetAsync(key);
        if (userSession == null)
            return null!;

        var ticket = ticketSerializer.Deserialize(userSession.Ticket);
        if (ticket != null)
            return ticket;

        // the ticket is bad, so remove from the store to clean up
        logger.LogWarning("Authentication ticket could not be deserialized. Deleting UserSession with key: {Key}",
            key);

        await RemoveAsync(key);

        return null!;
    }

    public Task RemoveAsync(string key)
    {
        return userSessionStore.DeleteAsync(key);
    }

    private UserSession CreateUserSession(string key, AuthenticationTicket ticket)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new InvalidOperationException("Invalid key");

        ArgumentNullException.ThrowIfNull(ticket);

        var subject = ticket.Principal.FindFirst(JwtClaimTypes.Subject)?.Value;
        var sessionId = ticket.Principal.FindFirst(JwtClaimTypes.SessionId)?.Value;

        return new UserSession(key, ticketSerializer.Serialize(ticket))
        {
            Subject = subject,
            SessionId = sessionId,
            Expiry = ticket.Properties.ExpiresUtc
        };
    }
}
