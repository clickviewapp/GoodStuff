namespace ClickView.GoodStuff.Repositories.MySql
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Abstractions;
    using Dapper;
    using Microsoft.Extensions.Logging;
    using MySqlConnector;
    using Polly;

    public abstract class BaseMySqlRepository : BaseRepository<MySqlConnection>
    {
        private readonly IAsyncPolicy _retryPolicy;
        private readonly ILogger<BaseMySqlRepository> _logger;

        protected BaseMySqlRepository(IMySqlConnectionFactory connectionFactory)
            : this(connectionFactory, new MySqlRepositoryOptions())
        {
        }

        protected BaseMySqlRepository(IMySqlConnectionFactory connectionFactory, MySqlRepositoryOptions options)
            : base(connectionFactory)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            // try to create a logger
            if (options.LoggerFactory != null)
                _logger = options.LoggerFactory.CreateLogger<BaseMySqlRepository>();

            _retryPolicy = Policy
                .Handle<MySqlException>(MySqlUtils.IsFailoverException)
                .WaitAndRetryAsync(options.FailOverRetryCount, options.FailOverRetryTimeout,
                    (ex, time) =>
                    {
                        // only log if we have a logger
                        if (_logger == null)
                            return;

                        // exception should always be the correct type as we are only handling MySqlExceptions
                        var mysqlEx = (MySqlException) ex;

                        _logger.LogWarning(ex,
                            "MySql query failed with error: {@Error}. Retrying in {RetrySeconds} seconds...",
                            new
                            {
                                Msg = mysqlEx.Message,
                                mysqlEx.Number,
                                HR = mysqlEx.HResult
                            },
                            time.TotalSeconds);
                    });
        }

        /// <summary>
        /// Executes a write command
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        protected Task<int> ExecuteAsync(string sql, object param = null)
        {
            return WrapAsync((c, s, p) => c.ExecuteAsync(s, p),
                write: true,
                sql: sql,
                param: param);
        }

        /// <summary>
        /// Executes a write command which selects a single value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        protected Task<T> ExecuteScalarAsync<T>(string sql, object param = null)
        {
            return WrapAsync((c, s, p) => c.ExecuteScalarAsync<T>(s, p),
                write: true,
                sql: sql,
                param: param);
        }

        /// <summary>
        /// Executes a single value query
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        protected Task<T> QueryScalarValueAsync<T>(string sql, object param = null)
        {
            return WrapAsync((c, s, p) => c.ExecuteScalarAsync<T>(s, p),
                write: false,
                sql: sql,
                param: param);
        }

        /// <summary>
        /// Executes a single row query
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        protected Task<T> QueryFirstAsync<T>(string sql, object param = null)
        {
            return WrapAsync((c, s, p) => c.QueryFirstOrDefaultAsync<T>(s, p),
                write: false,
                sql: sql,
                param: param);
        }

        /// <summary>
        /// Executes a single row query and throws an exception if more than one record is found
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        protected Task<T> QuerySingleAsync<T>(string sql, object param = null)
        {
            return WrapAsync((c, s, p) => c.QuerySingleOrDefaultAsync<T>(s, p),
                write: false,
                sql: sql,
                param: param);
        }

        /// <summary>
        /// Executes a multiple row query
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        protected Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null)
        {
            return WrapAsync((c, s, p) => c.QueryAsync<T>(s, p),
                write: false,
                sql: sql,
                param: param);
        }

        private Task<T> WrapAsync<T>(Func<MySqlConnection, string, object, Task<T>> func,
            bool write, string sql, object param = null)
        {
            return _retryPolicy.ExecuteAsync(async () =>
            {
                #if NETSTANDARD2_1
                await using var connection = write ? GetWriteConnection() : GetReadConnection();
                #else
                // ReSharper disable once UseAwaitUsing
                using var connection = write ? GetWriteConnection() : GetReadConnection();
                #endif

                try
                {
                    return await func(connection, sql, param);
                }
                catch (MySqlException ex) when (MySqlUtils.IsFailoverException(ex))
                {
                    _logger?.LogWarning("Clearing current connection pool because a fail-over exception occurred");

                    // catch the fail-over exceptions, remove the connection from the Pool
                    await MySqlConnection.ClearPoolAsync(connection);

                    throw;
                }
            });
        }
    }
}
