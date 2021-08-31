namespace ClickView.GoodStuff.AspNetCore.Authentication
{
    using System;

    public class TokenValidatorOptions
    {
        public Uri? Authority { get; set; }
        public string? DefaultAudience { get; set; }
    }
}
