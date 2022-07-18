namespace ClickView.GoodStuff.Repositories.MySql.Tests
{
    using Xunit;

    public class MySqlConnectionOptionsTests
    {
        /// <summary>
        /// Check for sane defaults
        /// </summary>
        [Fact]
        public void GetConnectionString_Default_Sane()
        {
            var options = new MySqlConnectionOptions();

            var connString = options.GetConnectionString();

            Assert.Equal("host=localhost;maxpoolsize=25;minpoolsize=1;", connString);
        }

        /// <summary>
        /// Check when all options set
        /// </summary>
        [Fact]
        public void GetConnectionString_AllOptionsSet_Valid()
        {
            var options = new MySqlConnectionOptions
            {
                Host = "why",
                Database = "do you",
                MaximumPoolSize = 11,
                MinimumPoolSize = 2,
                Password = "only call me",
                Port = 8888,
                Username = "when you're high?",
                LoadBalance = LoadBalance.LeastConnections,
                Pipelining = false
            };

            var connString = options.GetConnectionString();

            Assert.Equal(
                "host=why;" +
                "maxpoolsize=11;" +
                "minpoolsize=2;" +
                "database=do you;" +
                "password=only call me;" +
                "port=8888;" +
                "username=when you're high?;" +
                "loadbalance=leastconnections;" +
                "pipelining=false;",
                connString);
        }

        [Fact]
        public void PropertiesSet_AreEqual()
        {
            var options = new MySqlConnectionOptions
            {
                Host = "hello",
                Database = "db",
                Password = "pw",
                Port = 123,
                Username = "user",
                Pipelining = true,
                LoadBalance = LoadBalance.LeastConnections,
                CommandTimeout = 111,
                MaximumPoolSize = 222,
                MinimumPoolSize = 333
            };

            Assert.Equal("hello", options.Host);
            Assert.Equal("db", options.Database);
            Assert.Equal("pw", options.Password);
            Assert.Equal((ushort?)123, options.Port);
            Assert.Equal("user", options.Username);
            Assert.Equal(true, options.Pipelining);
            Assert.Equal(LoadBalance.LeastConnections, options.LoadBalance);
            Assert.Equal(111, options.CommandTimeout);
            Assert.Equal(222, options.MaximumPoolSize);
            Assert.Equal(333, options.MinimumPoolSize);
        }

        [Fact]
        public void PropertiesDefault_DoesNotThrow()
        {
            var options = new MySqlConnectionOptions();

            Assert.Equal("localhost", options.Host);
            Assert.Equal(25, options.MaximumPoolSize);
            Assert.Equal(1, options.MinimumPoolSize);

            Assert.Null(options.Database);
            Assert.Null(options.Password);
            Assert.Null(options.Port);
            Assert.Null(options.Username);
            Assert.Null(options.Pipelining);
            Assert.Null(options.LoadBalance);
            Assert.Null(options.CommandTimeout);
        }
    }
}
