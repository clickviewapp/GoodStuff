namespace ClickView.GoodStuff.Repositories.MySql
{
    using System;
    using Microsoft.Extensions.Logging;

    public class MySqlRepositoryOptions
    {
        /// <summary>
        /// The number of times to retry a query on a fail-over error
        /// </summary>
        public int FailOverRetryCount { get; set; } = 0;

        /// <summary>
        /// The function that provides the duration to wait for for a particular fail-over retry attempt
        /// </summary>
        public Func<int, TimeSpan> FailOverRetryTimeout { get; set; } = _ => TimeSpan.FromSeconds(2);

        public ILoggerFactory? LoggerFactory { get; set; }
    }
}
