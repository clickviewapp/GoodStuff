namespace ClickView.GoodStuff.AspNetCore.Authentication
{
    using System.Security.Claims;
    using System.Threading.Tasks;

    public interface ITokenValidator
    {
        Task<ClaimsPrincipal> ValidateLogoutTokenAsync(string logoutToken);
        Task<ClaimsPrincipal> ValidateLogoutTokenAsync(string logoutToken, string? validAudience);
    }
}
