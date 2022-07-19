namespace ClickView.GoodStuff.Repositories.MsSql.Tests
{
    using Xunit;

    public class MsSqlConnectionOptionsTests
    {
        /// <summary>
        /// Check for sane defaults
        /// </summary>
        [Fact]
        public void GetConnectionString_Default_Sane()
        {
            var options = new MsSqlConnectionOptions();

            var connString = options.GetConnectionString();

            Assert.Equal("Server=localhost;Trusted_Connection=True;TrustServerCertificate=True;Encrypt=True;" +
                         "Integrated Security=False;",
                         connString);
        }

        /// <summary>
        /// Check when all options set
        /// </summary>
        [Fact]
        public void GetConnectionString_AllOptionsSet_Valid()
        {
            var options = new MsSqlConnectionOptions
            {
                Host = "why:3306",
                Database = "do you",
                Password = "only call me",
                Username = "when you're high?",
            };

            var connString = options.GetConnectionString();

            Assert.Equal(
                "Server=why:3306;Trusted_Connection=True;TrustServerCertificate=True;Encrypt=True;" +
                "Integrated Security=False;Database=do you;Password=only call me;User ID=when you're high?;",
                connString);
        }

        [Fact]
        public void PropertiesSet_AreEqual()
        {
            var options = new MsSqlConnectionOptions
            {
                Host = "hello",
                Database = "db",
                Password = "pw",
                Username = "user",
            };

            Assert.Equal("hello", options.Host);
            Assert.Equal("db", options.Database);
            Assert.Equal("pw", options.Password);
            Assert.Equal("user", options.Username);
        }

        [Fact]
        public void PropertiesDefault_DoesNotThrow()
        {
            var options = new MsSqlConnectionOptions();

            Assert.Equal("localhost", options.Host);

            Assert.Null(options.Database);
            Assert.Null(options.Password);
            Assert.Null(options.Username);
        }
    }
}
