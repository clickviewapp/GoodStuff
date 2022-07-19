namespace ClickView.GoodStuff.Repositories.MySql
{
    using Abstractions;

    public class MySqlConnectionOptions : RepositoryConnectionOptions
    {
        public MySqlConnectionOptions()
        {
            // Set some sane defaults
            Host = "localhost";
            MaximumPoolSize = 25;
            MinimumPoolSize = 1;

            // Disable pipelining for now as Aurora does not support it
            SetParameter("pipelining", "false");
        }

        /// <summary>
        /// The TCP port on which Server is listening for connections.
        /// </summary>
        public ushort? Port
        {
            set => SetParameter("port", value?.ToString());
        }

        /// <summary>
        /// The case-sensitive name of the initial database to use
        /// </summary>
        public string? Database
        {
            set => SetParameter("database", value);
        }

        /// <summary>
        /// The MySQL user ID
        /// </summary>
        public string? Username
        {
            set => SetParameter("username", value);
        }

        /// <summary>
        /// The password for the MySQL user
        /// </summary>
        public string? Password
        {
            set => SetParameter("password", value);
        }

        /// <summary>
        /// The maximum number of connections allowed in the pool.
        /// </summary>
        public int? MaximumPoolSize
        {
            set => SetParameter("maxpoolsize", value?.ToString());
        }

        /// <summary>
        /// The minimum number of connections to leave in the pool if ConnectionIdleTimeout is reached
        /// </summary>
        public int? MinimumPoolSize
        {
            set => SetParameter("minpoolsize", value?.ToString());
        }

        /// <summary>
        /// The load-balancing strategy to use when Host contains multiple, comma-delimited, host names
        /// </summary>
        public LoadBalance.LoadBalanceOption? LoadBalance
        {
            set => SetParameter("loadbalance", value?.Value);
        }

        /// <summary>
        /// The length of time (in seconds) each command can execute before timing out and throwing an exception, or zero to disable timeouts
        /// </summary>
        public int? CommandTimeout
        {
            set => SetParameter("command timeout", value?.ToString());
        }
    }
}
