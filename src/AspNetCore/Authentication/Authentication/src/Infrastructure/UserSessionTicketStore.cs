namespace ClickView.GoodStuff.AspNetCore.Authentication.Infrastructure
{
    using System;
    using System.Threading.Tasks;
    using Abstractions;
    using IdentityModel;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.Extensions.Logging;

    internal sealed class UserSessionTicketStore : IUserSessionTicketStore
    {
        private readonly IUserSessionStore _userSessionStore;
        private readonly IDataSerializer<AuthenticationTicket> _ticketSerializer;
        private readonly ILogger<UserSessionTicketStore> _logger;

        public UserSessionTicketStore(IUserSessionStore userSessionStore,
            IDataSerializer<AuthenticationTicket> ticketSerializer,
            ILogger<UserSessionTicketStore> logger)
        {
            _userSessionStore = userSessionStore;
            _ticketSerializer = ticketSerializer;
            _logger = logger;
        }

        public async Task<string> StoreAsync(AuthenticationTicket ticket)
        {
            var key = CryptoRandom.CreateUniqueId(format: CryptoRandom.OutputFormat.Hex);

            var userSession = CreateUserSession(key, ticket);

            // ensure that we don't re-add the same session
            if (userSession.SessionId != null)
                await _userSessionStore.DeleteBySessionIdAsync(userSession.SessionId);

            await _userSessionStore.AddAsync(userSession);

            return key;
        }

        public Task RenewAsync(string key, AuthenticationTicket ticket)
        {
            var userSession = CreateUserSession(key, ticket);

            return _userSessionStore.UpdateAsync(key, userSession);
        }

#if NET6_0_OR_GREATER
        public async Task<AuthenticationTicket?> RetrieveAsync(string key)
#else
        public async Task<AuthenticationTicket> RetrieveAsync(string key)
#endif
        {
            var userSession = await _userSessionStore.GetAsync(key);
            if (userSession == null)
                return null!;

            var ticket = _ticketSerializer.Deserialize(userSession.Ticket);
            if (ticket != null)
                return ticket;

            // the ticket is bad, so remove from the store to clean up
            _logger.LogWarning("Authentication ticket could not be deserialized. Deleting UserSession with key: {Key}",
                key);

            await RemoveAsync(key);

            return null!;
        }

        public Task RemoveAsync(string key)
        {
            return _userSessionStore.DeleteAsync(key);
        }

        private UserSession CreateUserSession(string key, AuthenticationTicket ticket)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new InvalidOperationException("Invalid key");

            ArgumentNullException.ThrowIfNull(ticket);

            var subject = ticket.Principal.FindFirst(JwtClaimTypes.Subject)?.Value;
            var sessionId = ticket.Principal.FindFirst(JwtClaimTypes.SessionId)?.Value;

            return new UserSession(key, _ticketSerializer.Serialize(ticket))
            {
                Subject = subject,
                SessionId = sessionId,
                Expiry = ticket.Properties.ExpiresUtc
            };
        }
    }
}
