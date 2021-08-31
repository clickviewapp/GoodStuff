namespace ClickView.GoodStuff.AspNetCore.Authentication
{
    using System;
    using System.Collections.Generic;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Security.Cryptography;
    using System.Threading.Tasks;
    using IdentityModel;
    using IdentityModel.Client;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.Tokens;
    using Newtonsoft.Json.Linq;
    using JsonWebKeySet = IdentityModel.Jwk.JsonWebKeySet;

    public class TokenValidator : ITokenValidator
    {
        private readonly ILogger<TokenValidator> _logger;
        private readonly TokenValidatorOptions _options;
        private readonly DiscoveryCache _discoveryCache;

        private const string BackChannelScheme = "http://schemas.openid.net/event/backchannel-logout";

        public TokenValidator(IOptions<TokenValidatorOptions> options, ILogger<TokenValidator> logger)
        {
            var o = options.Value ?? throw new ArgumentNullException(nameof(options));

            if (o.Authority == null)
                throw new ArgumentException("Authority must be set");

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

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

            if (claims.FindFirst("sub") == null && claims.FindFirst("sid") == null)
                throw new Exception("Invalid logout token. sub or sid missing");

            var nonce = claims.FindFirst("nonce")?.Value;
            if (!string.IsNullOrWhiteSpace(nonce))
                throw new Exception("Invalid logout token. nonce missing");

            var eventsJson = claims.FindFirst("events")?.Value;
            if (string.IsNullOrWhiteSpace(eventsJson))
                throw new Exception("Invalid logout token. events missing");

            var events = JObject.Parse(eventsJson);

            if (events.TryGetValue(BackChannelScheme, out var logoutTokenData))
                return claims;

            _logger.LogWarning("Invalid backchannel logout token {LogoutTokenData}", logoutTokenData);

            throw new Exception("Invalid logout token");
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
