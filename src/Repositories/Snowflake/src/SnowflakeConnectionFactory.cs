namespace ClickView.GoodStuff.Repositories.Snowflake;

using Abstractions.Factories;
using global::Snowflake.Data.Client;

public class SnowflakeConnectionFactory(ConnectionFactoryOptions<SnowflakeConnectionOptions> options)
    : SnowflakeConnectionFactory<SnowflakeConnectionOptions>(options);

public class SnowflakeConnectionFactory<TOptions>(ConnectionFactoryOptions<TOptions> options)
    : ConnectionFactory<SnowflakeDbConnection, TOptions>(options), ISnowflakeConnectionFactory
    where TOptions : SnowflakeConnectionOptions
{
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
