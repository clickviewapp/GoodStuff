namespace ClickView.GoodStuff.AspNetCore.Authentication.Infrastructure
{
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.Extensions.Options;

    internal sealed class PostCookieAuthenticationOptions : IPostConfigureOptions<CookieAuthenticationOptions>
    {
        private readonly IUserSessionTicketStore _ticketStore;

        public PostCookieAuthenticationOptions(IUserSessionTicketStore ticketStore)
        {
            _ticketStore = ticketStore;
        }

        public void PostConfigure(string? name, CookieAuthenticationOptions options)
        {
            options.SessionStore = _ticketStore;
        }
    }
}
