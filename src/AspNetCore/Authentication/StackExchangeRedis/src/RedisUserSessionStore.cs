namespace ClickView.GoodStuff.AspNetCore.Authentication.StackExchangeRedis
{
    using Abstractions;
    using Microsoft.Extensions.Options;
    using StackExchange.Redis;
    using System;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;

    public class RedisUserSessionStore : IUserSessionStore
    {
        private readonly IDatabase _database;
        private readonly TimeSpan? _expiry;

        public RedisUserSessionStore(IOptions<RedisUserSessionCacheOptions> cacheOptions)
        {
            if (cacheOptions == null) throw new ArgumentException(nameof(cacheOptions));

            var options = cacheOptions.Value;

            if(options == null) throw new ArgumentNullException(nameof(options));

            if (options.Connection == null) throw new ArgumentException(nameof(options.Connection));

            _database = options.Connection.GetDatabase();
            _expiry = options.CacheExpiry;
        }

        public Task<UserSession?> GetAsync(string key, CancellationToken token = default)
        {
            return GetInternalAsync(key, token);
        }

        public Task AddAsync(UserSession session, CancellationToken token = default)
        {
            return AddInternalAsync(session.Key, session, token);
        }

        public async Task UpdateAsync(string key, UserSession session, CancellationToken token = default)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (session == null)
                throw new ArgumentNullException(nameof(session));

            token.ThrowIfCancellationRequested();

            var oldSession = await GetInternalAsync(key, token);
            await RemoveSessionIdAsync(oldSession, token);

            await AddInternalAsync(key, session, token);
        }

        public async Task DeleteAsync(string key, CancellationToken token = default)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            token.ThrowIfCancellationRequested();

            var session = await GetInternalAsync(key, token);

            if (await _database.KeyDeleteAsync(key))
                await RemoveSessionIdAsync(session, token);
        }

        public async Task DeleteBySessionIdAsync(string sessionId, CancellationToken token = default)
        {
            if (sessionId == null)
                throw new ArgumentNullException(nameof(sessionId));

            token.ThrowIfCancellationRequested();

            var sessionKey = await _database.StringGetAsync(GetSessionIdKey(sessionId));

            if (!sessionKey.HasValue) return;

            await _database.KeyDeleteAsync(sessionKey.ToString());

            //delete the session id lookup
            await _database.KeyDeleteAsync(GetSessionIdKey(sessionId));
        }

        public async Task<UserSession?> GetInternalAsync(string key, CancellationToken token = default)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            token.ThrowIfCancellationRequested();

            var session = await _database.StringGetAsync(key);
            return session.HasValue ? JsonSerializer.Deserialize<UserSession>(session.ToString()) : null;
        }

        public async Task AddInternalAsync(string key, UserSession session, CancellationToken token = default)
        {
            if (session == null)
                throw new ArgumentNullException(nameof(session));

            token.ThrowIfCancellationRequested();

            if (await _database.StringSetAsync(key, JsonSerializer.Serialize(session), _expiry))
                await AddSessionIdAsync(session, token);
        }

        private Task AddSessionIdAsync(UserSession? session, CancellationToken token = default)
        {
            if (string.IsNullOrWhiteSpace(session?.SessionId))
                return Task.CompletedTask;

            token.ThrowIfCancellationRequested();

            return _database.StringSetAsync(GetSessionIdKey(session.SessionId), session.Key, _expiry);
        }

        private Task RemoveSessionIdAsync(UserSession? session, CancellationToken token = default)
        {
            if (string.IsNullOrWhiteSpace(session?.SessionId))
                return Task.CompletedTask;

            token.ThrowIfCancellationRequested();

            return _database.KeyDeleteAsync(GetSessionIdKey(session.SessionId));
        }

        private static string GetSessionIdKey(string sessionId)
        {
            return $"sessionId:{sessionId}";
        }
    }
}
