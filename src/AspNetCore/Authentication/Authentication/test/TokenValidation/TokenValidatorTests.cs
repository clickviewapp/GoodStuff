namespace ClickView.GoodStuff.AspNetCore.Authentication.Tests.TokenValidation;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Authentication.TokenValidation;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Xunit;

public class TokenValidatorTests
{
    private static (TokenValidator Validator, RsaSecurityKey Key) CreateValidator(
        string? issuer = TestLogoutTokens.Issuer)
    {
        var signingKey = TestLogoutTokens.CreateSigningKey();

        var configuration = new OpenIdConnectConfiguration { Issuer = issuer };
        configuration.SigningKeys.Add(signingKey);

        var configurationManager = new StaticConfigurationManager<OpenIdConnectConfiguration>(configuration);

        var options = new TokenValidatorOptions { DefaultAudience = TestLogoutTokens.Audience };

        var validator = new TokenValidator(options, configurationManager);

        return (validator, signingKey);
    }

    [Fact]
    public async Task ValidateLogoutTokenAsync_ValidToken_ReturnsClaimsPrincipal()
    {
        var (validator, key) = CreateValidator();

        var token = TestLogoutTokens.Create(key, events: TestLogoutTokens.ValidEvents);

        var principal = await validator.ValidateLogoutTokenAsync(token);

        Assert.Equal("user-1", principal.FindFirst("sub")?.Value);
        Assert.Equal("session-1", principal.FindFirst("sid")?.Value);
    }

    [Fact]
    public async Task ValidateLogoutTokenAsync_WrongSigningKey_Throws()
    {
        var (validator, _) = CreateValidator();
        var otherKey = TestLogoutTokens.CreateSigningKey();

        var token = TestLogoutTokens.Create(otherKey, events: TestLogoutTokens.ValidEvents);

        await Assert.ThrowsAsync<InvalidLogoutTokenException>(() => validator.ValidateLogoutTokenAsync(token));
    }

    [Fact]
    public async Task ValidateLogoutTokenAsync_WrongIssuer_Throws()
    {
        var (validator, key) = CreateValidator();

        var token = TestLogoutTokens.Create(key, issuer: "https://not-the-issuer.example.com",
            events: TestLogoutTokens.ValidEvents);

        await Assert.ThrowsAsync<InvalidLogoutTokenException>(() => validator.ValidateLogoutTokenAsync(token));
    }

    [Fact]
    public async Task ValidateLogoutTokenAsync_WrongAudience_Throws()
    {
        var (validator, key) = CreateValidator();

        var token = TestLogoutTokens.Create(key, audience: "someone-else", events: TestLogoutTokens.ValidEvents);

        await Assert.ThrowsAsync<InvalidLogoutTokenException>(() => validator.ValidateLogoutTokenAsync(token));
    }

    [Fact]
    public async Task ValidateLogoutTokenAsync_MissingSubAndSid_Throws()
    {
        var (validator, key) = CreateValidator();

        var token = TestLogoutTokens.Create(key, subject: null, sessionId: null,
            events: TestLogoutTokens.ValidEvents);

        await Assert.ThrowsAsync<InvalidLogoutTokenException>(() => validator.ValidateLogoutTokenAsync(token));
    }

    [Fact]
    public async Task ValidateLogoutTokenAsync_SubOnly_Succeeds()
    {
        var (validator, key) = CreateValidator();

        var token = TestLogoutTokens.Create(key, sessionId: null, events: TestLogoutTokens.ValidEvents);

        var principal = await validator.ValidateLogoutTokenAsync(token);

        Assert.Equal("user-1", principal.FindFirst("sub")?.Value);
    }

    [Fact]
    public async Task ValidateLogoutTokenAsync_NoncePresent_Throws()
    {
        var (validator, key) = CreateValidator();

        var token = TestLogoutTokens.Create(key, nonce: "should-not-be-here", events: TestLogoutTokens.ValidEvents);

        await Assert.ThrowsAsync<InvalidLogoutTokenException>(() => validator.ValidateLogoutTokenAsync(token));
    }

    [Fact]
    public async Task ValidateLogoutTokenAsync_MissingEvents_Throws()
    {
        var (validator, key) = CreateValidator();

        var token = TestLogoutTokens.Create(key, events: null);

        await Assert.ThrowsAsync<InvalidLogoutTokenException>(() => validator.ValidateLogoutTokenAsync(token));
    }

    [Fact]
    public async Task ValidateLogoutTokenAsync_EventsMissingBackChannelKey_Throws()
    {
        var (validator, key) = CreateValidator();

        var token = TestLogoutTokens.Create(key,
            events: new Dictionary<string, object> { ["some-other-event"] = new Dictionary<string, object>() });

        await Assert.ThrowsAsync<InvalidLogoutTokenException>(() => validator.ValidateLogoutTokenAsync(token));
    }

    // Per the back-channel logout spec the event value is an empty JSON object, not a string -
    // this is the exact shape that the old Duende.IdentityModel TryGetString check needs to accept too.
    [Fact]
    public async Task ValidateLogoutTokenAsync_EventsBackChannelKeyWithEmptyObjectValue_Succeeds()
    {
        var (validator, key) = CreateValidator();

        var token = TestLogoutTokens.Create(key, events: TestLogoutTokens.ValidEvents);

        var principal = await validator.ValidateLogoutTokenAsync(token);

        Assert.NotNull(principal);
    }

    [Fact]
    public async Task ValidateLogoutTokenAsync_EventsBackChannelValueNotAnObject_Throws()
    {
        var (validator, key) = CreateValidator();

        var token = TestLogoutTokens.Create(key,
            events: new Dictionary<string, object> { [TestLogoutTokens.BackChannelScheme] = "not-an-object" });

        await Assert.ThrowsAsync<InvalidLogoutTokenException>(() => validator.ValidateLogoutTokenAsync(token));
    }

    [Fact]
    public async Task ValidateLogoutTokenAsync_MissingJti_Throws()
    {
        var (validator, key) = CreateValidator();

        var token = TestLogoutTokens.Create(key, jti: null, events: TestLogoutTokens.ValidEvents);

        await Assert.ThrowsAsync<InvalidLogoutTokenException>(() => validator.ValidateLogoutTokenAsync(token));
    }

    [Fact]
    public async Task ValidateLogoutTokenAsync_MissingExpiry_Throws()
    {
        var (validator, key) = CreateValidator();

        var token = TestLogoutTokens.Create(key, events: TestLogoutTokens.ValidEvents, includeExpiry: false);

        await Assert.ThrowsAsync<InvalidLogoutTokenException>(() => validator.ValidateLogoutTokenAsync(token));
    }

    [Fact]
    public async Task ValidateLogoutTokenAsync_Expired_Throws()
    {
        var (validator, key) = CreateValidator();

        var token = TestLogoutTokens.Create(key, events: TestLogoutTokens.ValidEvents,
            expires: DateTime.UtcNow.AddMinutes(-10));

        await Assert.ThrowsAsync<InvalidLogoutTokenException>(() => validator.ValidateLogoutTokenAsync(token));
    }
}
