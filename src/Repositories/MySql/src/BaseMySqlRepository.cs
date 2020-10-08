namespace ClickView.GoodStuff.Repositories.MySql
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Abstractions;
    using Dapper;
    using MySqlConnector;

    public abstract class BaseMySqlRepository : BaseRepository<MySqlConnection>
    {
        protected BaseMySqlRepository(IMySqlConnectionFactory connectionFactory) : base(connectionFactory)
        {
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

        private async Task<T> WrapAsync<T>(Func<MySqlConnection, string, object, Task<T>> func,
            bool write, string sql, object param = null)
        {
            await using var connection = write ? GetWriteConnection() : GetReadConnection();

            try
            {
                return await func(connection, sql, param);
            }
            catch (MySqlException ex) when (MySqlUtils.IsFailoverException(ex))
            {
                // catch the failover exceptions, remove the connection from the Pool
                await MySqlConnection.ClearPoolAsync(connection);

                throw;
            }
        }
    }
}
