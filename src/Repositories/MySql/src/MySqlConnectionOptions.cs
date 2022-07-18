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
        }

        /// <summary>
        /// The TCP port on which Server is listening for connections.
        /// </summary>
        public ushort? Port
        {
            set => SetParameter("port", value?.ToString());
            get => GetParameter<ushort>("port");
        }

        /// <summary>
        /// The case-sensitive name of the initial database to use
        /// </summary>
        public string? Database
        {
            set => SetParameter("database", value);
            get => GetParameter("database");
        }

        /// <summary>
        /// The MySQL user ID
        /// </summary>
        public string? Username
        {
            set => SetParameter("username", value);
            get => GetParameter("username");
        }

        /// <summary>
        /// The password for the MySQL user
        /// </summary>
        public string? Password
        {
            set => SetParameter("password", value);
            get => GetParameter("password");
        }

        /// <summary>
        /// The maximum number of connections allowed in the pool.
        /// </summary>
        public int? MaximumPoolSize
        {
            set => SetParameter("maxpoolsize", value?.ToString());
            get => GetParameter<int>("maxpoolsize");
        }

        /// <summary>
        /// The minimum number of connections to leave in the pool if ConnectionIdleTimeout is reached
        /// </summary>
        public int? MinimumPoolSize
        {
            set => SetParameter("minpoolsize", value?.ToString());
            get => GetParameter<int>("minpoolsize");
        }

        /// <summary>
        /// The load-balancing strategy to use when Host contains multiple, comma-delimited, host names
        /// </summary>
        public LoadBalance? LoadBalance
        {
            set => SetParameter("loadbalance", value?.ToString().ToLowerInvariant());
            get => GetParameter<LoadBalance>("loadbalance");
        }

        /// <summary>
        /// The length of time (in seconds) each command can execute before timing out and throwing an exception, or zero to disable timeouts
        /// </summary>
        public int? CommandTimeout
        {
            set => SetParameter("command timeout", value?.ToString());
            get => GetParameter<int>("command timeout");
        }

        /// <summary>
        /// Enables query pipelining
        /// </summary>
        public bool? Pipelining
        {
            set => SetParameter("pipelining", value?.ToString().ToLowerInvariant());
            get => GetParameter<bool>("pipelining");
        }
    }
}
