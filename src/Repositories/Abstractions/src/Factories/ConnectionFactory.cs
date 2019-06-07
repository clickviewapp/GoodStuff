namespace ClickView.GoodStuff.Repositories.Abstractions.Factories
{
    using System;

    public abstract class ConnectionFactory<TConnection, TOptions> : IConnectionFactory<TConnection>
        where TOptions : RepositoryConnectionOptions
    {
        protected readonly string ReadConnectionString;
        protected readonly string WriteConnectionString;

        protected ConnectionFactory(ConnectionFactoryOptions<TOptions> options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            if (options.Read == null && options.Write == null)
                throw new InvalidOperationException("At least one read or write connection options are required");

            ReadConnectionString = options.Read?.GetConnectionString();
            WriteConnectionString = options.Write?.GetConnectionString();
        }

        public abstract TConnection GetReadConnection();

        public abstract TConnection GetWriteConnection();
    }
}