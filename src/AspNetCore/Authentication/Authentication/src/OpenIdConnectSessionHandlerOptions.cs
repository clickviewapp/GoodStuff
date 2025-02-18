namespace ClickView.GoodStuff.AspNetCore.Authentication;

using System;

public class OpenIdConnectSessionHandlerOptions
{
    public Uri? Authority { get; set; }
    public string? DefaultAudience { get; set; }

    public void Validate()
    {
        if (Authority == null)
            throw new ArgumentException("Authority must be set", nameof(Authority));
    }
}
