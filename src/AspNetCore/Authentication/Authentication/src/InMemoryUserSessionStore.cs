﻿namespace ClickView.GoodStuff.AspNetCore.Authentication;

using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Abstractions;

public class InMemoryUserSessionStore : IUserSessionStore
{
    private readonly ConcurrentDictionary<string, UserSession> _sessions = new();

    public Task<UserSession?> GetAsync(string key, CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(key);

        token.ThrowIfCancellationRequested();

        _sessions.TryGetValue(key, out var session);

        if (session == null)
            return Task.FromResult<UserSession?>(null);

        if (session.Expiry != null && session.Expiry <= DateTimeOffset.UtcNow)
        {
            // Session expired. Remove from the store and return null
            _sessions.TryRemove(key, out _);
            return Task.FromResult<UserSession?>(null);
        }

        return Task.FromResult<UserSession?>(session);
    }

    public Task AddAsync(UserSession session, CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(session);

        token.ThrowIfCancellationRequested();

        _sessions.TryAdd(session.Key, session);

        return Task.CompletedTask;
    }

    public Task UpdateAsync(string key, UserSession session, CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(session);

        token.ThrowIfCancellationRequested();

        _sessions[key] = session;

        return Task.CompletedTask;
    }

    public Task DeleteAsync(string key, CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(key);

        token.ThrowIfCancellationRequested();

        _sessions.TryRemove(key, out _);

        return Task.CompletedTask;
    }

    public Task DeleteBySessionIdAsync(string sessionId, CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(sessionId);

        token.ThrowIfCancellationRequested();

        foreach (var item in _sessions.Values.Where(s => s.SessionId != null && s.SessionId.Equals(sessionId)))
        {
            _sessions.TryRemove(item.Key, out _);
        }

        return Task.CompletedTask;
    }
}
