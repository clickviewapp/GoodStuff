namespace ClickView.GoodStuff.AspNetCore
{
    using System;

    public sealed class ApplicationInformation
    {
        public ApplicationInformation(string name, string version)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Version = version ?? throw new ArgumentNullException(nameof(version));
        }

        public string Name { get; }
        public string Version { get; }
    }
}
