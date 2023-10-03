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

        public RedisUserSessionStore(IOptions<RedisUserSessionCacheOptions> cacheOptions)
        {
            ArgumentNullException.ThrowIfNull(cacheOptions);

            var options = cacheOptions.Value;

            if (options == null) throw new ArgumentException("Options cannot be null");
            if (options.Connection == null) throw new ArgumentException("Connection cannot be null");

            _database = options.Connection.GetDatabase();
        }

        public Task<UserSession?> GetAsync(string key, CancellationToken token = default)
        {
            return GetInternalAsync(key, token);
        }

        public async Task AddAsync(UserSession session, CancellationToken token = default)
        {
            ArgumentNullException.ThrowIfNull(session);

            token.ThrowIfCancellationRequested();

            // Create a transaction so we set both values at the same time
            var transaction = _database.CreateTransaction();

            AddInternal(transaction, session.Key, session);

            await transaction.ExecuteAsync();
        }

        public async Task UpdateAsync(string key, UserSession session, CancellationToken token = default)
        {
            ArgumentNullException.ThrowIfNull(key);
            ArgumentNullException.ThrowIfNull(session);

            token.ThrowIfCancellationRequested();

            var oldSession = await GetInternalAsync(key, token);

            var transaction = _database.CreateTransaction();

            // if we have an existing session, delete the id key
            if (oldSession?.SessionId != null)
                _ = transaction.KeyDeleteAsync(GetSessionIdKey(oldSession.SessionId));

            AddInternal(transaction, key, session);

            await transaction.ExecuteAsync();
        }

        public async Task DeleteAsync(string key, CancellationToken token = default)
        {
            ArgumentNullException.ThrowIfNull(key);

            token.ThrowIfCancellationRequested();

            var session = await GetInternalAsync(key, token);

            var keysToDelete = session?.SessionId == null
                ? new RedisKey[] {key}
                : new RedisKey[] {key, GetSessionIdKey(session.SessionId)};

            // Delete both the session and the session id lookup
            await _database.KeyDeleteAsync(keysToDelete);
        }

        public async Task DeleteBySessionIdAsync(string sessionId, CancellationToken token = default)
        {
            ArgumentNullException.ThrowIfNull(sessionId);

            token.ThrowIfCancellationRequested();

            var sessionKey = await _database.StringGetAsync(GetSessionIdKey(sessionId));

            if (!sessionKey.HasValue) return;

            var keys = new RedisKey[]
            {
                sessionKey.ToString(),
                GetSessionIdKey(sessionId)
            };

            // Delete both the session and the session id lookup
            await _database.KeyDeleteAsync(keys);
        }

        private async Task<UserSession?> GetInternalAsync(string key, CancellationToken token = default)
        {
            ArgumentNullException.ThrowIfNull(key);

            token.ThrowIfCancellationRequested();

            var session = await _database.StringGetAsync(key);
            return session.HasValue ? Deserialize(session!) : null;
        }

        private static void AddInternal(ITransaction transaction, string key, UserSession session)
        {
            var expireTimeSpan = session.Expiry?.ToRedisExpiryTimeSpan();

            _ = transaction.StringSetAsync(key, Serialize(session), expireTimeSpan);

            if (!string.IsNullOrWhiteSpace(session.SessionId))
                _ = transaction.StringSetAsync(GetSessionIdKey(session.SessionId), session.Key, expireTimeSpan);
        }

        private static UserSession? Deserialize(byte[] value) => JsonSerializer.Deserialize<UserSession>(value);
        private static byte[] Serialize(UserSession userSession) => JsonSerializer.SerializeToUtf8Bytes(userSession);

        private static string GetSessionIdKey(string sessionId) => "sessionId:" + sessionId;
    }
}
