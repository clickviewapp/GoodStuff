namespace ClickView.GoodStuff.Repositories.MySql.Tests
{
    using System;
    using Abstractions.Factories;
    using Xunit;

    public class MySqlConnectionFactoryTests
    {
        [Fact]
        public void GetReadConnection()
        {
            var factory = new MySqlConnectionFactory(new ConnectionFactoryOptions<MySqlConnectionOptions>
            {
                Read = new MySqlConnectionOptions()
            });

            var readConn = factory.GetReadConnection();

            Assert.NotNull(readConn);
        }

        [Fact]
        public void GetWriteConnection()
        {
            var factory = new MySqlConnectionFactory(new ConnectionFactoryOptions<MySqlConnectionOptions>
            {
                Write = new MySqlConnectionOptions()
            });

            var writeConn = factory.GetWriteConnection();

            Assert.NotNull(writeConn);
        }

        [Fact]
        public void Ctor_NullOptions_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new MySqlConnectionFactory(null));
        }

        [Fact]
        public void Ctor_NullReadAndWrite_ThrowsInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() =>
                new MySqlConnectionFactory(new ConnectionFactoryOptions<MySqlConnectionOptions>()));
        }
    }
}