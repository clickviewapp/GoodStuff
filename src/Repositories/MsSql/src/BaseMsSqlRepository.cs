namespace ClickView.GoodStuff.Repositories.MsSql
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Abstractions;
    using Dapper;
    using Microsoft.Data.SqlClient;

    public abstract class BaseMsSqlRepository : BaseRepository<SqlConnection>
    {
        protected BaseMsSqlRepository(IMsSqlConnectionFactory connectionFactory) : base(connectionFactory)
        {
        }

        /// <summary>
        /// Executes a write command
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        protected async Task<int> ExecuteAsync(string sql, object? param = null)
        {
            using var conn = GetWriteConnection();
            return await conn.ExecuteAsync(sql, param);
        }

        /// <summary>
        /// Executes a write command which selects a single value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        protected async Task<T?> ExecuteScalarAsync<T>(string sql, object? param = null)
        {
            using var conn = GetWriteConnection();
            return await conn.ExecuteScalarAsync<T>(sql, param);
        }

        /// <summary>
        /// Executes a single value query
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        protected async Task<T?> QueryScalarValueAsync<T>(string sql, object? param = null)
        {
            using var conn = GetReadConnection();
            return await conn.ExecuteScalarAsync<T>(sql, param);
        }

        /// <summary>
        /// Executes a single row query
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        protected async Task<T?> QueryFirstAsync<T>(string sql, object? param = null)
        {
            using var conn = GetReadConnection();
            return await conn.QueryFirstOrDefaultAsync<T>(sql, param);
        }

        /// <summary>
        /// Executes a single row query and throws an exception if more than one record is found
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        protected async Task<T?> QuerySingleAsync<T>(string sql, object? param = null)
        {
            using var conn = GetReadConnection();
            return await conn.QuerySingleOrDefaultAsync<T>(sql, param);
        }

        /// <summary>
        /// Executes a multiple row query
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        protected async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null)
        {
            using var conn = GetReadConnection();
            return await conn.QueryAsync<T>(sql, param);
        }
    }
}
