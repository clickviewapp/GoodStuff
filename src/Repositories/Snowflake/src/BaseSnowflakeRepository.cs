namespace ClickView.GoodStuff.Repositories.Snowflake;

using System.Threading;
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
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected Task<int> ExecuteAsync(string sql, object? param = null, CancellationToken cancellationToken = default)
    {
        return WrapAsync((con, cd) => con.ExecuteAsync(cd),
            write: true,
            sql,
            param,
            cancellationToken);
    }

    /// <summary>
    /// Executes a write command which selects a single value
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="sql"></param>
    /// <param name="param"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected Task<T?> ExecuteScalarAsync<T>(string sql, object? param = null,
        CancellationToken cancellationToken = default)
    {
        return WrapAsync((con, cd) => con.ExecuteScalarAsync<T>(cd),
            write: true,
            sql,
            param,
            cancellationToken);
    }

    /// <summary>
    /// Executes a single value query
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="sql"></param>
    /// <param name="param"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected Task<T?> QueryScalarValueAsync<T>(string sql, object? param = null,
        CancellationToken cancellationToken = default)
    {
        return WrapAsync((con, cd) => con.ExecuteScalarAsync<T>(cd),
            write: false,
            sql,
            param,
            cancellationToken);
    }

    /// <summary>
    /// Executes a single row query
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="sql"></param>
    /// <param name="param"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? param = null,
        CancellationToken cancellationToken = default)
    {
        return WrapAsync((con, cd) => con.QueryFirstOrDefaultAsync<T>(cd),
            write: false,
            sql,
            param,
            cancellationToken);
    }

    /// <summary>
    /// Executes a single row query and throws an exception if more than one record is found
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="sql"></param>
    /// <param name="param"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected Task<T?> QuerySingleOrDefaultAsync<T>(string sql, object? param = null,
        CancellationToken cancellationToken = default)
    {
        return WrapAsync((con, cd) => con.QuerySingleOrDefaultAsync<T>(cd),
            write: false,
            sql,
            param,
            cancellationToken);
    }

    /// <summary>
    /// Executes a multiple row query
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="sql"></param>
    /// <param name="param"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null,
        CancellationToken cancellationToken = default)
    {
        return WrapAsync((con, cd) => con.QueryAsync<T>(cd),
            write: false,
            sql,
            param,
            cancellationToken);
    }

    protected Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TReturn>(string sql,
        Func<TFirst, TSecond, TThird, TReturn> map, object? param = null, string splitOn = "Id",
        CancellationToken cancellationToken = default)
    {
        return WrapAsync((con, cd) => con.QueryAsync(cd, map, splitOn: splitOn),
            write: false,
            sql,
            param,
            cancellationToken);
    }

    private async Task<T> WrapAsync<T>(Func<SnowflakeDbConnection, CommandDefinition, Task<T>> func,
        bool write, string sql, object? param, CancellationToken cancellationToken)
    {
#if NETSTANDARD2_1_OR_GREATER || NET6_0_OR_GREATER
        await using var conn = write ? GetWriteConnection() : GetReadConnection();
#else
        using var conn = write ? GetWriteConnection() : GetReadConnection();
#endif

        var command = new CommandDefinition(sql, param, cancellationToken: cancellationToken);

        return await func(conn, command);
    }
}
