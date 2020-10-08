namespace ClickView.GoodStuff.Repositories.MsSql.Tests
{
    using System;
    using Abstractions.Factories;
    using Xunit;

    public class MsSqlConnectionFactoryTests
    {
        [Fact]
        public void GetReadConnection()
        {
            var factory = new MySqlConnectionFactory(new ConnectionFactoryOptions<MsSqlConnectionOptions>
            {
                Read = new MsSqlConnectionOptions()
            });

            var readConn = factory.GetReadConnection();

            Assert.NotNull(readConn);
        }

        [Fact]
        public void GetWriteConnection()
        {
            var factory = new MySqlConnectionFactory(new ConnectionFactoryOptions<MsSqlConnectionOptions>
            {
                Write = new MsSqlConnectionOptions()
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
                new MySqlConnectionFactory(new ConnectionFactoryOptions<MsSqlConnectionOptions>()));
        }
    }
}
