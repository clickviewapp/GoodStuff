namespace ClickView.GoodStuff.AspNetCore.Authentication.Abstractions
{
    using System;

    public sealed class UserSession
    {
        public UserSession(string key, byte[] ticket)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
            Ticket = ticket ?? throw new ArgumentNullException(nameof(ticket));
        }

        public string Key { get; }
        public byte[] Ticket { get; }
        public string? SessionId { get; set; }
        public string? Subject { get; set; }

        /// <summary>
        /// The time at which the user session expires
        /// </summary>
        public DateTimeOffset? Expiry { get; set; }
    }
}
