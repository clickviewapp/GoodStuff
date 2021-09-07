namespace ClickView.GoodStuff.AspNetCore.Authentication.StackExchangeRedis
{
    using System;

    public static class DateTimeOffSetExtensions
    {
        public static TimeSpan? ToUtcTimeSpan(this DateTimeOffset? value)
        {
            if (value == null) return null;

            var dif = value.Value.Subtract(DateTimeOffset.UtcNow);

            if (dif.TotalSeconds <= 0)
                throw new Exception("Time already expired");

            return dif;
        }
    }
}
