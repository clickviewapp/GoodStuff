namespace ClickView.GoodStuff.AspNetCore.Authentication.TokenValidation;

using System.Security.Claims;
using System.Threading.Tasks;

internal interface ITokenValidator
{
    Task<ClaimsPrincipal> ValidateLogoutTokenAsync(string logoutToken, CancellationToken cancellationToken = default);
    Task<ClaimsPrincipal> ValidateLogoutTokenAsync(string logoutToken, string? validAudience, CancellationToken cancellationToken = default);
}
