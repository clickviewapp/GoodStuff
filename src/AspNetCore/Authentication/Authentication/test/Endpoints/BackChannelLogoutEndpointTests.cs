namespace ClickView.GoodStuff.AspNetCore.Authentication.Tests.Endpoints;

using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Abstractions;
using Authentication.Endpoints;
using Authentication.TokenValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Primitives;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

public class BackChannelLogoutEndpointTests
{
    private static DefaultHttpContext CreateFormContext(string? logoutToken)
    {
        var context = new DefaultHttpContext
        {
            Request =
            {
                ContentType = "application/x-www-form-urlencoded",
                Form = logoutToken == null
                    ? new FormCollection(new Dictionary<string, StringValues>())
                    : new FormCollection(new Dictionary<string, StringValues> {["logout_token"] = logoutToken})
            }
        };

        return context;
    }

    [Fact]
    public async Task ProcessAsync_NotFormContent_Returns400()
    {
        var tokenValidator = Substitute.For<ITokenValidator>();
        var sessionStore = Substitute.For<IUserSessionStore>();

        var endpoint = new BackChannelLogoutEndpoint(tokenValidator, sessionStore,
            NullLogger<BackChannelLogoutEndpoint>.Instance);

        var context = new DefaultHttpContext();

        await endpoint.ProcessAsync(context, CancellationToken.None);

        Assert.Equal(400, context.Response.StatusCode);
        await sessionStore.DidNotReceiveWithAnyArgs().DeleteBySessionIdAsync(null!);
    }

    [Fact]
    public async Task ProcessAsync_MissingLogoutTokenField_Returns400()
    {
        var tokenValidator = Substitute.For<ITokenValidator>();
        var sessionStore = Substitute.For<IUserSessionStore>();

        var endpoint = new BackChannelLogoutEndpoint(tokenValidator, sessionStore,
            NullLogger<BackChannelLogoutEndpoint>.Instance);

        var context = CreateFormContext(logoutToken: null);

        await endpoint.ProcessAsync(context, CancellationToken.None);

        Assert.Equal(400, context.Response.StatusCode);
        await tokenValidator.DidNotReceiveWithAnyArgs().ValidateLogoutTokenAsync(null!);
    }

    [Fact]
    public async Task ProcessAsync_ValidatorThrows_Returns400()
    {
        var tokenValidator = Substitute.For<ITokenValidator>();
        tokenValidator.ValidateLogoutTokenAsync(Arg.Any<string>())
            .ThrowsAsync(new Exception("invalid token"));

        var sessionStore = Substitute.For<IUserSessionStore>();

        var endpoint = new BackChannelLogoutEndpoint(tokenValidator, sessionStore,
            NullLogger<BackChannelLogoutEndpoint>.Instance);

        var context = CreateFormContext("some-token");

        await endpoint.ProcessAsync(context, CancellationToken.None);

        Assert.Equal(400, context.Response.StatusCode);
        await sessionStore.DidNotReceiveWithAnyArgs().DeleteBySessionIdAsync(null!);
    }

    [Fact]
    public async Task ProcessAsync_MissingSessionIdClaim_Returns400()
    {
        var tokenValidator = Substitute.For<ITokenValidator>();
        tokenValidator.ValidateLogoutTokenAsync(Arg.Any<string>())
            .Returns(new ClaimsPrincipal(new ClaimsIdentity([new Claim("sub", "user-1")])));

        var sessionStore = Substitute.For<IUserSessionStore>();

        var endpoint = new BackChannelLogoutEndpoint(tokenValidator, sessionStore,
            NullLogger<BackChannelLogoutEndpoint>.Instance);

        var context = CreateFormContext("some-token");

        await endpoint.ProcessAsync(context, CancellationToken.None);

        Assert.Equal(400, context.Response.StatusCode);
        await sessionStore.DidNotReceiveWithAnyArgs().DeleteBySessionIdAsync(null!);
    }

    [Fact]
    public async Task ProcessAsync_ValidToken_DeletesSessionAndDoesNotSetErrorStatus()
    {
        var tokenValidator = Substitute.For<ITokenValidator>();
        tokenValidator.ValidateLogoutTokenAsync(Arg.Any<string>())
            .Returns(new ClaimsPrincipal(new ClaimsIdentity(
            [
                new Claim("sub", "user-1"),
                new Claim("sid", "session-1")
            ])));

        var sessionStore = Substitute.For<IUserSessionStore>();

        var endpoint = new BackChannelLogoutEndpoint(tokenValidator, sessionStore,
            NullLogger<BackChannelLogoutEndpoint>.Instance);

        var context = CreateFormContext("some-token");

        await endpoint.ProcessAsync(context, CancellationToken.None);

        Assert.Equal(200, context.Response.StatusCode);
        await sessionStore.Received(1).DeleteBySessionIdAsync("session-1", CancellationToken.None);
    }
}
