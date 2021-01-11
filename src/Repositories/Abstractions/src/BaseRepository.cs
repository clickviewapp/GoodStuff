﻿namespace ClickView.GoodStuff.Repositories.Abstractions
{
    using System;
    using Factories;

    public abstract class BaseRepository<TConnection>
    {
        private readonly IConnectionFactory<TConnection> _connectionFactory;

        protected BaseRepository(IConnectionFactory<TConnection> connectionFactory)
        {
            if (connectionFactory == null)
                throw new ArgumentNullException(nameof(connectionFactory));

            _connectionFactory = connectionFactory;
        }

        /// <summary>
        /// Get a read connection
        /// </summary>
        /// <returns></returns>
        protected TConnection GetReadConnection()
        {
            return _connectionFactory.GetReadConnection();
        }

        /// <summary>
        /// Get a write connection
        /// </summary>
        /// <returns></returns>
        protected TConnection GetWriteConnection()
        {
            return _connectionFactory.GetWriteConnection();
        }
    }
}
