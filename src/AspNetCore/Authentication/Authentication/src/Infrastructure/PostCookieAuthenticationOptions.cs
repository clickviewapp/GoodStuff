namespace ClickView.GoodStuff.AspNetCore.Authentication.Infrastructure;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;

internal sealed class PostCookieAuthenticationOptions(IUserSessionTicketStore ticketStore)
    : IPostConfigureOptions<CookieAuthenticationOptions>
{
    public void PostConfigure(string? name, CookieAuthenticationOptions options)
    {
        options.SessionStore = ticketStore;
    }
}
