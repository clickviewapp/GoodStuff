namespace ClickView.GoodStuff.AspNetCore.Authentication.StackExchangeRedis
{
    using System;

    public static class DateTimeOffSetExtensions
    {
        public static TimeSpan? ToRedisExpiryTimeSpan(this DateTimeOffset? value)
        {
            if (value == null) return null;

            var dif = value.Value.Subtract(DateTimeOffset.UtcNow);

            //redis setex command expects the time to be positive
            if (dif.TotalSeconds <= 0)
                throw new Exception("Time already expired");

            return dif;
        }
    }
}
