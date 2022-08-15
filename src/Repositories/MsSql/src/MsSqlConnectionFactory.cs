namespace ClickView.GoodStuff.Repositories.MsSql
{
    using System;
    using System.Data.SqlClient;
    using Abstractions.Factories;

    public class MsSqlConnectionFactory : MsSqlConnectionFactory<MsSqlConnectionOptions>
    {
        public MsSqlConnectionFactory(ConnectionFactoryOptions<MsSqlConnectionOptions> options) : base(options)
        {
        }
    }

    public class MsSqlConnectionFactory<TOptions>
        : ConnectionFactory<SqlConnection, TOptions>, IMsSqlConnectionFactory
        where TOptions : MsSqlConnectionOptions
    {
        public MsSqlConnectionFactory(ConnectionFactoryOptions<TOptions> options) : base(options)
        {
        }

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
