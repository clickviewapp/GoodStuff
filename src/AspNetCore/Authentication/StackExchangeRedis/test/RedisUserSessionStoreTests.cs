namespace ClickView.GoodStuff.AspNetCore.Authentication.StackExchangeRedis.Tests
{
    using Abstractions;
    using Microsoft.Extensions.DependencyInjection;
    using StackExchange.Redis;
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Xunit;

    public class RedisUserSessionStoreTests
    {
        [Fact]
        public async Task Can_Add_UserSession_To_Redis()
        {
            var services = new ServiceCollection();

            //redis
            var redis = ConnectionMultiplexer.Connect("localhost:6379");

            services.AddRedisCacheUserSessionStore(options =>
            {
                options.Connection = redis;
            });

            // Build
            var serviceProvider = services.BuildServiceProvider();

            var session = new UserSession("1", new byte[] {1, 2})
            {
                SessionId = "sessionId1",
                Subject = "subject1",
                Expiry = DateTimeOffset.UtcNow.AddSeconds(10)
            };

            var sessionStore = serviceProvider.GetRequiredService<IUserSessionStore>();
            await sessionStore.AddAsync(session, CancellationToken.None);
            var retrievedSession = await sessionStore.GetAsync("1", CancellationToken.None);

            Assert.NotNull(retrievedSession);
            Assert.Equal(session.Key, retrievedSession.Key);
            Assert.Equal(session.Ticket, retrievedSession.Ticket);
            Assert.Equal(session.SessionId, retrievedSession.SessionId);
            Assert.Equal(session.Subject, retrievedSession.Subject);
        }

        [Fact]
        public async Task Can_Update_UserSession_To_Same_key()
        {
            var services = new ServiceCollection();

            //redis
            var redis = ConnectionMultiplexer.Connect("localhost:6379");

            services.AddRedisCacheUserSessionStore(options =>
            {
                options.Connection = redis;
            });

            // Build
            var serviceProvider = services.BuildServiceProvider();

            var session = new UserSession("2", new byte[] { 3, 4 })
            {
                SessionId = "sessionId2",
                Subject = "subject2",
                Expiry = DateTimeOffset.UtcNow.AddSeconds(10)
            };

            var sessionStore = serviceProvider.GetRequiredService<IUserSessionStore>();
            await sessionStore.AddAsync(session, CancellationToken.None);
            var retrievedSession = await sessionStore.GetAsync("2", CancellationToken.None);

            Assert.NotNull(retrievedSession);

            var newSession = new UserSession("2", new byte[] {5, 6})
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

        [Fact]
        public async Task Can_Delete_UserSession_By_key()
        {
            var services = new ServiceCollection();

            //redis
            var redis = ConnectionMultiplexer.Connect("localhost:6379");

            services.AddRedisCacheUserSessionStore(options =>
            {
                options.Connection = redis;
            });

            // Build
            var serviceProvider = services.BuildServiceProvider();

            var session = new UserSession("3", new byte[] { 7, 8 })
            {
                SessionId = "sessionId3",
                Subject = "subject3"
            };

            var sessionStore = serviceProvider.GetRequiredService<IUserSessionStore>();
            await sessionStore.AddAsync(session, CancellationToken.None);
            var retrievedSession = await sessionStore.GetAsync("3", CancellationToken.None);

            Assert.NotNull(retrievedSession);

            await sessionStore.DeleteAsync("3", CancellationToken.None);

            var nullSession = await sessionStore.GetAsync("3", CancellationToken.None);
            Assert.Null(nullSession);
        }

        [Fact]
        public async Task Can_Delete_UserSession_By_SessionId()
        {
            var services = new ServiceCollection();

            //redis
            var redis = ConnectionMultiplexer.Connect("localhost:6379");

            services.AddRedisCacheUserSessionStore(options =>
            {
                options.Connection = redis;
            });

            // Build
            var serviceProvider = services.BuildServiceProvider();

            var session = new UserSession("4", new byte[] { 9, 10 })
            {
                SessionId = "sessionId4",
                Subject = "subject4"
            };

            var sessionStore = serviceProvider.GetRequiredService<IUserSessionStore>();
            await sessionStore.AddAsync(session, CancellationToken.None);
            var retrievedSession = await sessionStore.GetAsync("4", CancellationToken.None);

            Assert.NotNull(retrievedSession);

            await sessionStore.DeleteBySessionIdAsync("sessionId4", CancellationToken.None);

            var nullSession = await sessionStore.GetAsync("4", CancellationToken.None);
            Assert.Null(nullSession);
        }

        [Fact]
        public async Task Can_Expire_UserSession_Key()
        {
            var services = new ServiceCollection();

            //redis
            var redis = ConnectionMultiplexer.Connect("localhost:6379");

            services.AddRedisCacheUserSessionStore(options =>
            {
                options.Connection = redis;
            });

            // Build
            var serviceProvider = services.BuildServiceProvider();

            var session = new UserSession("5", new byte[] { 1, 2 })
            {
                SessionId = "sessionId5",
                Subject = "subject5",
                Expiry = DateTimeOffset.UtcNow.AddSeconds(1)
            };

            var sessionStore = serviceProvider.GetRequiredService<IUserSessionStore>();
            await sessionStore.AddAsync(session, CancellationToken.None);
            var retrievedSession = await sessionStore.GetAsync("5", CancellationToken.None);

            Assert.NotNull(retrievedSession);

            //ensure we wait enough time for key to expire
            await Task.Delay(TimeSpan.FromSeconds(2));

            var retrievedSessionAgain = await sessionStore.GetAsync("5", CancellationToken.None);

            Assert.Null(retrievedSessionAgain);
        }

        [Fact]
        public async Task Add_Expired_Session_ThrowsException()
        {
            var ex = await Assert.ThrowsAsync<Exception>(async () =>
            {
                var services = new ServiceCollection();

                //redis
                var redis = ConnectionMultiplexer.Connect("localhost:6379");

                services.AddRedisCacheUserSessionStore(options =>
                {
                    options.Connection = redis;
                });

                // Build
                var serviceProvider = services.BuildServiceProvider();

                var session = new UserSession("6", new byte[] { 1, 2 })
                {
                    SessionId = "sessionId6",
                    Subject = "subject6",
                    Expiry = DateTimeOffset.UtcNow.AddSeconds(-10)
                };

                var sessionStore = serviceProvider.GetRequiredService<IUserSessionStore>();
                await sessionStore.AddAsync(session, CancellationToken.None);
            });
        }
    }
}
