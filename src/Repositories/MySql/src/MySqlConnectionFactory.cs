namespace ClickView.GoodStuff.Repositories.MySql
{
    using System;
    using Abstractions.Factories;
    using global::MySql.Data.MySqlClient;

    public class MySqlConnectionFactory : MySqlConnectionFactory<MySqlConnectionOptions>
    {
        public MySqlConnectionFactory(ConnectionFactoryOptions<MySqlConnectionOptions> options) : base(options)
        {
        }
    }

    public class MySqlConnectionFactory<TOptions> : ConnectionFactory<MySqlConnection, TOptions> where TOptions : MySqlConnectionOptions
    {
        public MySqlConnectionFactory(ConnectionFactoryOptions<TOptions> options) : base(options)
        {
        }

        public override MySqlConnection GetReadConnection()
        {
            if (string.IsNullOrEmpty(ReadConnectionString))
                throw new InvalidOperationException("Read is not allowed. No read connection options defined");

            return new MySqlConnection(ReadConnectionString);
        }

        public override MySqlConnection GetWriteConnection()
        {
            if (string.IsNullOrEmpty(WriteConnectionString))
                throw new InvalidOperationException("Write is not allowed. No write connection options defined");

            return new MySqlConnection(WriteConnectionString);
        }
    }
}
