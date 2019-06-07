namespace ClickView.GoodStuff.Repositories.MySql
{
    public static class LoadBalance
    {
        /// <summary>
        /// Each new connection opened for this connection pool uses the next host name (sequentially with wraparound)
        /// </summary>
        public static LoadBalanceOption RoundRobin = new LoadBalanceOption("roundrobin");

        /// <summary>
        /// Each new connection tries to connect to the first host; subsequent hosts are used only if connecting to the first one fails
        /// </summary>
        public static LoadBalanceOption FailOver = new LoadBalanceOption("failover");

        /// <summary>
        /// Servers are tried in a random order.
        /// </summary>
        public static LoadBalanceOption Random = new LoadBalanceOption("random");

        /// <summary>
        /// Servers are tried in ascending order of number of currently-open connections in this connection pool.
        /// </summary>
        public static LoadBalanceOption LeastConnections = new LoadBalanceOption("leastconnections");

        public struct LoadBalanceOption
        {
            public string Value { get; }

            internal LoadBalanceOption(string value)
            {
                Value = value;
            }
        }
    }
}