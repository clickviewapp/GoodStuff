namespace ClickView.GoodStuff.AspNetCore.Authentication.TokenValidation;

using System;
using System.Security.Claims;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

internal sealed class TokenValidator : ITokenValidator
{
    private static readonly JsonWebTokenHandler _handler = new();

    private readonly TokenValidatorOptions _options;
    private readonly IConfigurationManager<OpenIdConnectConfiguration> _configurationManager;

    private const string RoleClaimType = "role";
    private const string EventsClaimType = "events";
    private const string BackChannelScheme = "http://schemas.openid.net/event/backchannel-logout";

    public TokenValidator(IOptions<TokenValidatorOptions> options)
    {
        var o = options.Value ?? throw new ArgumentNullException(nameof(options));

        if (o.Authority == null)
            throw new ArgumentException("Authority must be set");

        _options = o;
        _configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
            new Uri(o.Authority, ".well-known/openid-configuration").AbsoluteUri,
            new OpenIdConnectConfigurationRetriever());
    }

    internal TokenValidator(TokenValidatorOptions options, IConfigurationManager<OpenIdConnectConfiguration> configurationManager)
    {
        _options = options;
        _configurationManager = configurationManager;
    }

    public Task<ClaimsPrincipal> ValidateLogoutTokenAsync(string logoutToken, CancellationToken cancellationToken = default)
    {
        return ValidateLogoutTokenAsync(logoutToken, _options.DefaultAudience, cancellationToken);
    }

    public async Task<ClaimsPrincipal> ValidateLogoutTokenAsync(string logoutToken, string? validAudience,
        CancellationToken cancellationToken = default)
    {
        var claims = await ValidateJwtAsync(logoutToken, validAudience, cancellationToken);

        if (claims.FindFirst(JwtRegisteredClaimNames.Sub) == null &&
            claims.FindFirst(JwtRegisteredClaimNames.Sid) == null)
        {
            throw new InvalidLogoutTokenException(
                $"Invalid logout token. {JwtRegisteredClaimNames.Sub} or {JwtRegisteredClaimNames.Sid} missing");
        }

        var nonce = claims.FindFirst(JwtRegisteredClaimNames.Nonce)?.Value;

        if (!string.IsNullOrWhiteSpace(nonce))
            throw new InvalidLogoutTokenException($"Invalid logout token. {JwtRegisteredClaimNames.Nonce} missing");

        if (claims.FindFirst(JwtRegisteredClaimNames.Jti) == null)
            throw new InvalidLogoutTokenException($"Invalid logout token. {JwtRegisteredClaimNames.Jti} missing");

        var eventsJson = claims.FindFirst(EventsClaimType)?.Value;

        if (string.IsNullOrWhiteSpace(eventsJson))
            throw new InvalidLogoutTokenException($"Invalid logout token. {EventsClaimType} missing");

        var events = JsonDocument.Parse(eventsJson).RootElement;

        if (!events.TryGetProperty(BackChannelScheme, out var backChannelEvent) ||
            backChannelEvent.ValueKind != JsonValueKind.Object)
        {
            throw new InvalidLogoutTokenException("Invalid logout token");
        }

        return claims;
    }

    private async Task<ClaimsPrincipal> ValidateJwtAsync(string jwt, string? validAudience,
        CancellationToken cancellationToken)
    {
        var openIdConnectConfiguration = await _configurationManager.GetConfigurationAsync(cancellationToken);

        var parameters = new TokenValidationParameters
        {
            ValidIssuer = openIdConnectConfiguration.Issuer,
            ValidAudience = validAudience,
            IssuerSigningKeys = openIdConnectConfiguration.SigningKeys,
            NameClaimType = JwtRegisteredClaimNames.Name,
            RoleClaimType = RoleClaimType
        };

        var result = await _handler.ValidateTokenAsync(jwt, parameters);

        if (!result.IsValid)
            throw new InvalidLogoutTokenException("Invalid logout token", result.Exception);

        return new ClaimsPrincipal(result.ClaimsIdentity);
    }
}
