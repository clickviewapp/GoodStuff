namespace ClickView.GoodStuff.AspNetCore.Authentication.TokenValidation
{
    using System;
    using System.Collections.Generic;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Security.Cryptography;
    using System.Text.Json;
    using System.Threading.Tasks;
    using IdentityModel;
    using IdentityModel.Client;
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.Tokens;
    using JsonWebKeySet = IdentityModel.Jwk.JsonWebKeySet;

    internal sealed class TokenValidator : ITokenValidator
    {
        private readonly TokenValidatorOptions _options;
        private readonly DiscoveryCache _discoveryCache;

        private const string BackChannelScheme = "http://schemas.openid.net/event/backchannel-logout";

        public TokenValidator(IOptions<TokenValidatorOptions> options)
        {
            var o = options.Value ?? throw new ArgumentNullException(nameof(options));

            if (o.Authority == null)
                throw new ArgumentException("Authority must be set");

            _options = o;
            _discoveryCache = new DiscoveryCache(o.Authority.AbsoluteUri);
        }

        public Task<ClaimsPrincipal> ValidateLogoutTokenAsync(string logoutToken)
        {
            return ValidateLogoutTokenAsync(logoutToken, _options.DefaultAudience);
        }

        public async Task<ClaimsPrincipal> ValidateLogoutTokenAsync(string logoutToken, string? validAudience)
        {
            var claims = await ValidateJwtAsync(logoutToken, validAudience);

            if (claims.FindFirst(JwtClaimTypes.Subject) == null && claims.FindFirst(JwtClaimTypes.SessionId) == null)
            {
                throw new Exception(
                    $"Invalid logout token. {JwtClaimTypes.Subject} or {JwtClaimTypes.SessionId} missing");
            }

            var nonce = claims.FindFirst(JwtClaimTypes.Nonce)?.Value;

            if (!string.IsNullOrWhiteSpace(nonce))
                throw new Exception($"Invalid logout token. {JwtClaimTypes.Nonce} missing");

            var eventsJson = claims.FindFirst(JwtClaimTypes.Events)?.Value;

            if (string.IsNullOrWhiteSpace(eventsJson))
                throw new Exception($"Invalid logout token. {JwtClaimTypes.Events} missing");

            var events = JsonDocument.Parse(eventsJson).RootElement;
            var logoutEvent = events.TryGetString(BackChannelScheme);

            if (logoutEvent == null)
                throw new Exception("Invalid logout token");

            return claims;
        }

        private async Task<ClaimsPrincipal> ValidateJwtAsync(string jwt, string? validAudience)
        {
            var disco = await _discoveryCache.GetAsync();

            var parameters = new TokenValidationParameters
            {
                ValidIssuer = disco.Issuer,
                ValidAudience = validAudience,
                IssuerSigningKeys = GetSecurityKeys(disco.KeySet),

                NameClaimType = JwtClaimTypes.Name,
                RoleClaimType = JwtClaimTypes.Role
            };

            var handler = new JwtSecurityTokenHandler();
            handler.InboundClaimTypeMap.Clear();

            return handler.ValidateToken(jwt, parameters, out _);
        }

        private static IEnumerable<SecurityKey> GetSecurityKeys(JsonWebKeySet keySet)
        {
            foreach (var webKey in keySet.Keys)
            {
                var e = Base64Url.Decode(webKey.E);
                var n = Base64Url.Decode(webKey.N);

                yield return new RsaSecurityKey(new RSAParameters {Exponent = e, Modulus = n})
                {
                    KeyId = webKey.Kid
                };
            }
        }
    }
}
