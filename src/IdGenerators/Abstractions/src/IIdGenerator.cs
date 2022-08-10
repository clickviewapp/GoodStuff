namespace ClickView.GoodStuff.IdGenerators.Abstractions;

using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Interface for creating ids
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IIdGenerator<T>
{
    /// <summary>
    /// Creates a new id
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<T> CreateAsync(CancellationToken token = default);
}
