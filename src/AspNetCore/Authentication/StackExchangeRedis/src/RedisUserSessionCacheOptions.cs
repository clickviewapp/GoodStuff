namespace ClickView.GoodStuff.AspNetCore.Authentication.StackExchangeRedis
{
    using StackExchange.Redis;
    using System;

    public class RedisUserSessionCacheOptions
    {
        public IConnectionMultiplexer? Connection { get; set; }
        public string? InstanceName { get; set; }

        public void Validate()
        {
            if (Connection == null)
                throw new ArgumentException("Redis Connection must be set", nameof(Connection));
        }
    }
}
