namespace ClickView.GoodStuff.AspNetCore.Authentication.StackExchangeRedis
{
    using StackExchange.Redis;
    using System;

    public class RedisUserSessionCacheOptions
    {
        public IConnectionMultiplexer? Connection { get; set; }
        public string? InstanceName { get; set; }
        public TimeSpan? CacheExpiry { get; set; }

        public void Validate()
        {
            if (Connection == null)
                throw new ArgumentException("Redis Connection must be set", nameof(Connection));

            if (string.IsNullOrWhiteSpace(InstanceName))
                throw new ArgumentException("InstanceName must be set", nameof(InstanceName));
        }
    }
}
