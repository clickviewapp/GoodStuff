namespace ClickView.GoodStuff.AspNetCore.Authentication.StackExchangeRedis
{
    using System;

    internal static class DateTimeOffSetExtensions
    {
        public static TimeSpan ToRedisExpiryTimeSpan(this DateTimeOffset value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            var dif = value.Subtract(DateTimeOffset.UtcNow);

            //redis setex command expects the time to be positive
            if (dif.TotalSeconds <= 0)
                throw new Exception("Time already expired");

            return dif;
        }
    }
}
