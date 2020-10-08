﻿namespace ClickView.GoodStuff.Repositories.MsSql
{
    using System;
    using System.Data.SqlClient;
    using Abstractions.Factories;

    public class MySqlConnectionFactory : MySqlConnectionFactory<MsSqlConnectionOptions>
    {
        public MySqlConnectionFactory(ConnectionFactoryOptions<MsSqlConnectionOptions> options) : base(options)
        {
        }
    }

    public class MySqlConnectionFactory<TOptions>
        : ConnectionFactory<SqlConnection, TOptions>, IMsSqlConnectionFactory
        where TOptions : MsSqlConnectionOptions
    {
        public MySqlConnectionFactory(ConnectionFactoryOptions<TOptions> options) : base(options)
        {
        }

        public override SqlConnection GetReadConnection()
        {
            if (string.IsNullOrEmpty(ReadConnectionString))
                throw new InvalidOperationException("Read is not allowed. No read connection options defined");

            return new SqlConnection(ReadConnectionString);
        }

        public override SqlConnection GetWriteConnection()
        {
            if (string.IsNullOrEmpty(WriteConnectionString))
                throw new InvalidOperationException("Write is not allowed. No write connection options defined");

            return new SqlConnection(WriteConnectionString);
        }
    }
}
