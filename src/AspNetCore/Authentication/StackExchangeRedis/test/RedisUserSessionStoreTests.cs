namespace ClickView.GoodStuff.AspNetCore.Authentication.StackExchangeRedis.Tests;

using Abstractions;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System;
using System.Threading;
using System.Threading.Tasks;
using GoodStuff.Tests.xUnit;
using McMaster.Extensions.Xunit;
using Xunit;

public class RedisUserSessionStoreTests
{
    private const string SkipCiReason = "Redis server is needed";

    [SkippableFact]
    [SkipOnCI(SkipCiReason)]
    public async Task Can_Add_UserSession_To_Redis()
    {
        var session = new UserSession("1", [1, 2])
        {
            SessionId = "sessionId1",
            Subject = "subject1",
            Expiry = DateTimeOffset.UtcNow.AddSeconds(10)
        };

        var sessionStore = await GetUserSessionStoreAsync();

        await sessionStore.AddAsync(session, CancellationToken.None);
        var retrievedSession = await sessionStore.GetAsync("1", CancellationToken.None);

        Assert.NotNull(retrievedSession);
        Assert.Equal(session.Key, retrievedSession.Key);
        Assert.Equal(session.Ticket, retrievedSession.Ticket);
        Assert.Equal(session.SessionId, retrievedSession.SessionId);
        Assert.Equal(session.Subject, retrievedSession.Subject);
    }

    [SkippableFact]
    [SkipOnCI(SkipCiReason)]
    public async Task AddAsync_InstanceName()
    {
        var session = new UserSession("1", [1, 2])
        {
            SessionId = "sessionId1",
            Subject = "subject1",
            Expiry = DateTimeOffset.UtcNow.AddSeconds(10)
        };

        var sessionStore = await GetUserSessionStoreAsync(o =>
        {
            o.InstanceName = "test_";
        });

        await sessionStore.AddAsync(session, CancellationToken.None);
        var retrievedSession = await sessionStore.GetAsync("1", CancellationToken.None);

        Assert.NotNull(retrievedSession);
        Assert.Equal(session.Key, retrievedSession.Key);
        Assert.Equal(session.Ticket, retrievedSession.Ticket);
        Assert.Equal(session.SessionId, retrievedSession.SessionId);
        Assert.Equal(session.Subject, retrievedSession.Subject);
    }

    [SkippableFact]
    [SkipOnCI(SkipCiReason)]
    public async Task Can_Update_UserSession_To_Same_key()
    {
        var session = new UserSession("2", [3, 4])
        {
            SessionId = "sessionId2",
            Subject = "subject2",
            Expiry = DateTimeOffset.UtcNow.AddSeconds(10)
        };

        var sessionStore = await GetUserSessionStoreAsync();

        await sessionStore.AddAsync(session, CancellationToken.None);
        var retrievedSession = await sessionStore.GetAsync("2", CancellationToken.None);

        Assert.NotNull(retrievedSession);

        var newSession = new UserSession("2", [5, 6])
        {
            SessionId = "session3",
            Subject = "subject3",
            Expiry = DateTimeOffset.UtcNow.AddSeconds(10)
        };

        await sessionStore.UpdateAsync("2", newSession, CancellationToken.None);
        var updatedSession = await sessionStore.GetAsync("2", CancellationToken.None);

        Assert.NotNull(updatedSession);

        Assert.Equal(session.Key, updatedSession.Key);

        Assert.NotEqual(session.Ticket, updatedSession.Ticket);
        Assert.NotEqual(session.SessionId, updatedSession.SessionId);
        Assert.NotEqual(session.Subject, updatedSession.Subject);
    }

    [SkippableFact]
    [SkipOnCI(SkipCiReason)]
    public async Task Can_Delete_UserSession_By_key()
    {
        var session = new UserSession("3", [7, 8])
        {
            SessionId = "sessionId3",
            Subject = "subject3"
        };

        var sessionStore = await GetUserSessionStoreAsync();

        await sessionStore.AddAsync(session, CancellationToken.None);
        var retrievedSession = await sessionStore.GetAsync("3", CancellationToken.None);

        Assert.NotNull(retrievedSession);

        await sessionStore.DeleteAsync("3", CancellationToken.None);

        var nullSession = await sessionStore.GetAsync("3", CancellationToken.None);
        Assert.Null(nullSession);
    }

    [SkippableFact]
    [SkipOnCI(SkipCiReason)]
    public async Task Can_Delete_UserSession_By_SessionId()
    {
        var session = new UserSession("4", [9, 10])
        {
            SessionId = "sessionId4",
            Subject = "subject4"
        };

        var sessionStore = await GetUserSessionStoreAsync();

        await sessionStore.AddAsync(session, CancellationToken.None);
        var retrievedSession = await sessionStore.GetAsync("4", CancellationToken.None);

        Assert.NotNull(retrievedSession);

        await sessionStore.DeleteBySessionIdAsync("sessionId4", CancellationToken.None);

        var nullSession = await sessionStore.GetAsync("4", CancellationToken.None);
        Assert.Null(nullSession);
    }

    [SkippableFact]
    [SkipOnCI(SkipCiReason)]
    public async Task Can_Expire_UserSession_Key()
    {
        var session = new UserSession("5", [1, 2])
        {
            SessionId = "sessionId5",
            Subject = "subject5",
            Expiry = DateTimeOffset.UtcNow.AddSeconds(1)
        };

        var sessionStore = await GetUserSessionStoreAsync();

        await sessionStore.AddAsync(session, CancellationToken.None);
        var retrievedSession = await sessionStore.GetAsync("5", CancellationToken.None);

        Assert.NotNull(retrievedSession);

        //ensure we wait enough time for key to expire
        await Task.Delay(TimeSpan.FromSeconds(2));

        var retrievedSessionAgain = await sessionStore.GetAsync("5", CancellationToken.None);

        Assert.Null(retrievedSessionAgain);
    }

    [SkippableFact]
    [SkipOnCI(SkipCiReason)]
    public async Task Add_Expired_Session_ThrowsException()
    {
        await Assert.ThrowsAsync<Exception>(async () =>
        {
            var session = new UserSession("6", [1, 2])
            {
                SessionId = "sessionId6",
                Subject = "subject6",
                Expiry = DateTimeOffset.UtcNow.AddSeconds(-10)
            };

            var sessionStore = await GetUserSessionStoreAsync();

            await sessionStore.AddAsync(session, CancellationToken.None);
        });
    }

    private static async Task<IUserSessionStore> GetUserSessionStoreAsync(
        Action<RedisUserSessionCacheOptions>? configure = null)
    {
        var services = new ServiceCollection();
        var builder = new OpenIdConnectSessionHandlerBuilder(services);

        //redis
        var redis = await ConnectionMultiplexer.ConnectAsync("localhost:6379");

        builder.AddStackExchangeRedisUserSessionStore(options =>
        {
            options.Connection = redis;

            configure?.Invoke(options);
        });

        // Build
        var serviceProvider = services.BuildServiceProvider();
        return serviceProvider.GetRequiredService<IUserSessionStore>();
    }
}
