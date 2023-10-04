namespace ClickView.GoodStuff.Repositories.MySql
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Abstractions;
    using Dapper;
    using Microsoft.Extensions.Logging;
    using MySqlConnector;
    using Polly;

    public abstract class BaseMySqlRepository : BaseRepository<MySqlConnection>
    {
        private readonly IAsyncPolicy _retryPolicy;
        private readonly ILogger<BaseMySqlRepository>? _logger;

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
            if (options.LoggerFactory is not null)
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
        /// <param name="token"></param>
        /// <returns></returns>
        protected Task<int> ExecuteAsync(string sql, object? param = null, CancellationToken token = default)
        {
            return WrapAsync((con, cd) => con.ExecuteAsync(cd),
                write: true,
                sql: sql,
                param: param,
                token: token);
        }

        /// <summary>
        /// Executes a write command which selects a single value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        protected Task<T?> ExecuteScalarAsync<T>(string sql, object? param = null, CancellationToken token = default)
        {
            return WrapAsync((con, cd) => con.ExecuteScalarAsync<T>(cd),
                write: true,
                sql: sql,
                param: param,
                token: token);
        }

        /// <summary>
        /// Executes a single value query
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        protected Task<T?> QueryScalarValueAsync<T>(string sql, object? param = null, CancellationToken token = default)
        {
            return WrapAsync((con, cd) => con.ExecuteScalarAsync<T>(cd),
                write: false,
                sql: sql,
                param: param,
                token: token);
        }

        /// <summary>
        /// Executes a single row query
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        protected Task<T?> QueryFirstAsync<T>(string sql, object? param = null, CancellationToken token = default)
        {
            return WrapAsync((con, cd) => con.QueryFirstOrDefaultAsync<T>(cd),
                write: false,
                sql: sql,
                param: param,
                token: token);
        }

        /// <summary>
        /// Executes a single row query and throws an exception if more than one record is found
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        protected Task<T?> QuerySingleAsync<T>(string sql, object? param = null, CancellationToken token = default)
        {
            return WrapAsync((con, cd) => con.QuerySingleOrDefaultAsync<T>(cd),
                write: false,
                sql: sql,
                param: param,
                token: token);
        }

        /// <summary>
        /// Executes a multiple row query
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        protected Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null, CancellationToken token = default)
        {
            return WrapAsync((con, cd) => con.QueryAsync<T>(cd),
                write: false,
                sql: sql,
                param: param,
                token: token);
        }

        private Task<T> WrapAsync<T>(Func<MySqlConnection, CommandDefinition, Task<T>> func,
            bool write, string sql, object? param, CancellationToken token)
        {
            return _retryPolicy.ExecuteAsync(async cancellationToken =>
            {
#if NETSTANDARD2_1_OR_GREATER || NET6_0_OR_GREATER
                await using var connection = write ? GetWriteConnection() : GetReadConnection();
#else
                // ReSharper disable once UseAwaitUsing
                using var connection = write ? GetWriteConnection() : GetReadConnection();
#endif

                try
                {
                    var command = new CommandDefinition(sql, param, cancellationToken: cancellationToken);

                    return await func(connection, command);
                }
                catch (MySqlException ex) when (MySqlUtils.IsFailoverException(ex))
                {
                    _logger?.LogWarning("Clearing current connection pool because a fail-over exception occurred");

                    // catch the fail-over exceptions, remove the connection from the Pool
                    await MySqlConnection.ClearPoolAsync(connection, cancellationToken);

                    throw;
                }
            }, token);
        }
    }
}
