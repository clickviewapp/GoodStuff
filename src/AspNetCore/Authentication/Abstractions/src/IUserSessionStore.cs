namespace ClickView.GoodStuff.AspNetCore.Authentication.Abstractions;

using System.Threading;
using System.Threading.Tasks;

public interface IUserSessionStore
{
    Task<UserSession?> GetAsync(string key, CancellationToken token = default);
    Task AddAsync(UserSession session, CancellationToken token = default);
    Task UpdateAsync(string key, UserSession session, CancellationToken token = default);
    Task DeleteAsync(string key, CancellationToken token = default);
    Task DeleteBySessionIdAsync(string sessionId, CancellationToken token = default);
}
