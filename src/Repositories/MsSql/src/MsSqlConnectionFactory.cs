namespace ClickView.GoodStuff.Repositories.MsSql
{
    using System;
    using Abstractions.Factories;
    using Microsoft.Data.SqlClient;

    public class MsSqlConnectionFactory(ConnectionFactoryOptions<MsSqlConnectionOptions> options)
        : MsSqlConnectionFactory<MsSqlConnectionOptions>(options);

    public class MsSqlConnectionFactory<TOptions>(ConnectionFactoryOptions<TOptions> options)
        : ConnectionFactory<SqlConnection, TOptions>(options), IMsSqlConnectionFactory
        where TOptions : MsSqlConnectionOptions
    {
        public override SqlConnection GetReadConnection()
        {
            var cs = ReadConnectionString;

            if (string.IsNullOrEmpty(cs))
                throw new InvalidOperationException("Read is not allowed. No read connection options defined");

            return new SqlConnection(cs);
        }

        public override SqlConnection GetWriteConnection()
        {
            var cs = WriteConnectionString;

            if (string.IsNullOrEmpty(cs))
                throw new InvalidOperationException("Write is not allowed. No write connection options defined");

            return new SqlConnection(cs);
        }
    }
}
