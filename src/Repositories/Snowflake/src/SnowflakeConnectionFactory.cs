namespace ClickView.GoodStuff.Repositories.Snowflake
{
    using System;
    using Abstractions.Factories;
    using global::Snowflake.Data.Client;

    public class SnowflakeConnectionFactory : SnowflakeConnectionFactory<SnowflakeConnectionOptions>
    {
        public SnowflakeConnectionFactory(ConnectionFactoryOptions<SnowflakeConnectionOptions> options) : base(options)
        {
        }
    }

    public class SnowflakeConnectionFactory<TOptions>
        : ConnectionFactory<SnowflakeDbConnection, TOptions>, ISnowflakeConnectionFactory
        where TOptions : SnowflakeConnectionOptions
    {
        public SnowflakeConnectionFactory(ConnectionFactoryOptions<TOptions> options) : base(options)
        {
        }

        public override SnowflakeDbConnection GetReadConnection()
        {
            if (string.IsNullOrEmpty(ReadConnectionString))
                throw new InvalidOperationException("Read is not allowed. No read connection options defined");

            var connection = new SnowflakeDbConnection { ConnectionString = ReadConnectionString };
            connection.Open();

            return connection;
        }

        public override SnowflakeDbConnection GetWriteConnection()
        {
            if (string.IsNullOrEmpty(WriteConnectionString))
                throw new InvalidOperationException("Write is not allowed. No write connection options defined");

            var connection = new SnowflakeDbConnection { ConnectionString = WriteConnectionString };
            connection.Open();
            
            return connection;
        }
    }
}