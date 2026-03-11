namespace ClickView.GoodStuff.Repositories.Snowflake;

using Abstractions;
using Dapper;
using global::Snowflake.Data.Client;

public abstract class BaseSnowflakeRepository(ISnowflakeConnectionFactory connectionFactory)
    : BaseRepository<SnowflakeDbConnection>(connectionFactory)
{
    /// <summary>
    /// Executes a write command
    /// </summary>
    /// <param name="sql"></param>
    /// <param name="param"></param>
    /// <returns></returns>
    protected async Task<int> ExecuteAsync(string sql, object? param = null)
    {
#if NETSTANDARD2_1_OR_GREATER || NET6_0_OR_GREATER
        await using var conn = GetWriteConnection();
#else
        using var conn = GetWriteConnection();
#endif
            
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
#if NETSTANDARD2_1_OR_GREATER || NET6_0_OR_GREATER
        await using var conn = GetWriteConnection();
#else
            using var conn = GetWriteConnection();
#endif

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
#if NETSTANDARD2_1_OR_GREATER || NET6_0_OR_GREATER
        await using var conn = GetReadConnection();
#else
            using var conn = GetReadConnection();
#endif

        return await conn.ExecuteScalarAsync<T>(sql, param);
    }

    /// <summary>
    /// Executes a single row query
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="sql"></param>
    /// <param name="param"></param>
    /// <returns></returns>
    protected async Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? param = null)
    {
#if NETSTANDARD2_1_OR_GREATER || NET6_0_OR_GREATER
        await using var conn = GetReadConnection();
#else
            using var conn = GetReadConnection();
#endif

        return await conn.QueryFirstOrDefaultAsync<T>(sql, param);
    }

    /// <summary>
    /// Executes a single row query and throws an exception if more than one record is found
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="sql"></param>
    /// <param name="param"></param>
    /// <returns></returns>
    protected async Task<T?> QuerySingleOrDefaultAsync<T>(string sql, object? param = null)
    {
#if NETSTANDARD2_1_OR_GREATER || NET6_0_OR_GREATER
        await using var conn = GetReadConnection();
#else
            using var conn = GetReadConnection();
#endif

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
#if NETSTANDARD2_1_OR_GREATER || NET6_0_OR_GREATER
        await using var conn = GetReadConnection();
#else
            using var conn = GetReadConnection();
#endif

        return await conn.QueryAsync<T>(sql, param);
    }

    protected async Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TReturn>(string sql,
        Func<TFirst, TSecond, TThird, TReturn> map, object? param = null, string splitOn = "Id")
    {
#if NETSTANDARD2_1_OR_GREATER || NET6_0_OR_GREATER
        await using var conn = GetReadConnection();
#else
            using var conn = GetReadConnection();
#endif

        return await conn.QueryAsync(sql, map, splitOn: splitOn, param: param);
    }
}
