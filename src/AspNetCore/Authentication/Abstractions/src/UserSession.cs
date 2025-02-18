namespace ClickView.GoodStuff.AspNetCore.Authentication.Abstractions;

using System;

public sealed class UserSession(string key, byte[] ticket)
{
    public string Key { get; } = key ?? throw new ArgumentNullException(nameof(key));
    public byte[] Ticket { get; } = ticket ?? throw new ArgumentNullException(nameof(ticket));
    public string? SessionId { get; set; }
    public string? Subject { get; set; }

    /// <summary>
    /// The time at which the user session expires
    /// </summary>
    public DateTimeOffset? Expiry { get; set; }
}
