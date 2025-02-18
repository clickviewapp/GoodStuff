namespace ClickView.GoodStuff.AspNetCore.Authentication.TokenValidation;

using System;

internal sealed class TokenValidatorOptions
{
    public Uri? Authority { get; set; }
    public string? DefaultAudience { get; set; }
}
