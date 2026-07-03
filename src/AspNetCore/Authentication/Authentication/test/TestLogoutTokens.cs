namespace ClickView.GoodStuff.AspNetCore.Authentication.Tests;

using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

internal static class TestLogoutTokens
{
    public const string Issuer = "https://issuer.example.com";
    public const string Audience = "test-client";
    public const string BackChannelScheme = "http://schemas.openid.net/event/backchannel-logout";

    private static readonly JsonWebTokenHandler _handler = new();

    public static object ValidEvents => new Dictionary<string, object>
    {
        [BackChannelScheme] = new Dictionary<string, object>()
    };

    public static RsaSecurityKey CreateSigningKey(string keyId = "test-key")
    {
        return new RsaSecurityKey(RSA.Create(2048)) { KeyId = keyId };
    }

    public static string Create(
        RsaSecurityKey signingKey,
        string? issuer = Issuer,
        string? audience = Audience,
        string? subject = "user-1",
        string? sessionId = "session-1",
        string? nonce = null,
        string? jti = "logout-token-1",
        object? events = null,
        bool includeExpiry = true,
        DateTime? expires = null)
    {
        var claims = new Dictionary<string, object>();

        if (subject != null)
            claims[JwtRegisteredClaimNames.Sub] = subject;

        if (sessionId != null)
            claims[JwtRegisteredClaimNames.Sid] = sessionId;

        if (nonce != null)
            claims[JwtRegisteredClaimNames.Nonce] = nonce;

        if (jti != null)
            claims[JwtRegisteredClaimNames.Jti] = jti;

        if (events != null)
            claims["events"] = events;

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = issuer,
            Audience = audience,
            IssuedAt = DateTime.UtcNow,
            Expires = expires,
            Claims = claims,
            SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.RsaSha256)
        };

        // A dedicated handler instance is used (rather than the shared one) so that suppressing
        // the default 'exp' claim for one token can't race with other tests sharing Handler.
        var handler = includeExpiry ? _handler : new JsonWebTokenHandler { SetDefaultTimesOnTokenCreation = false };

        return handler.CreateToken(descriptor);
    }
}
