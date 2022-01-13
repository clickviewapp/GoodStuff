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

            Assert.Equal("host=localhost;maxpoolsize=25;minpoolsize=1;pipelining=false;", connString);
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
                LoadBalance = LoadBalance.LeastConnections
            };

            var connString = options.GetConnectionString();

            Assert.Equal(
                "host=why;maxpoolsize=11;minpoolsize=2;pipelining=false;database=do you;password=only call me;port=8888;username=when you're high?;loadbalance=leastconnections;",
                connString);
        }
    }
}
