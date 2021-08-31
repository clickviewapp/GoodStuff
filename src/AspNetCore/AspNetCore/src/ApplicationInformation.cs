namespace ClickView.GoodStuff.AspNetCore
{
    using System;
    using System.Diagnostics;
    using System.Reflection;

    public sealed class ApplicationInformation
    {
        public ApplicationInformation(string name, string version)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Value cannot be empty", nameof(name));

            if (string.IsNullOrWhiteSpace(version))
                throw new ArgumentException("Value cannot be empty", nameof(version));

            Name = name;
            Version = version;
        }

        public string Name { get; }
        public string Version { get; }

        public static ApplicationInformation FromAssembly(Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));

            return FromFileVersion(FileVersionInfo.GetVersionInfo(assembly.Location));
        }

        public static ApplicationInformation FromFileVersion(FileVersionInfo fileVersionInfo)
        {
            if (fileVersionInfo == null)
                throw new ArgumentNullException(nameof(fileVersionInfo));

            if (string.IsNullOrWhiteSpace(fileVersionInfo.ProductName))
            {
                throw new ArgumentException(
                    nameof(FileVersionInfo.ProductName) + " cannot be empty",
                    nameof(fileVersionInfo));
            }

            if (string.IsNullOrWhiteSpace(fileVersionInfo.ProductVersion))
            {
                throw new ArgumentException(
                    nameof(FileVersionInfo.ProductVersion) + " cannot be empty",
                    nameof(fileVersionInfo));
            }

            return new ApplicationInformation(fileVersionInfo.ProductName, fileVersionInfo.ProductVersion);
        }
    }
}
