namespace ClickView.GoodStuff.Configuration.Vault;

using Microsoft.Extensions.Configuration;

public static class ConfigurationManagerExtensions
{
    private const string VaultSectionKey = "Vault";
    private const string UrlKey = "Url";
    private const string TokenKey = "Token";
    private const string PathKey = "Path";
    private const string MountPointKey = "MountPoint";
    private const string VersionKey = "Version";

    public static IConfigurationManager AddVault(this IConfigurationManager configurationManager,
        Action<VaultOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(configurationManager);

        var options = GetOptions(configurationManager);

        configure?.Invoke(options);

        // Validate required properties
        if (string.IsNullOrEmpty(options.Url))
            throw new VaultConfigurationException("Url is required");

        if (string.IsNullOrEmpty(options.Token))
            throw new VaultConfigurationException("Token is required");

        if (string.IsNullOrEmpty(options.Path))
            throw new VaultConfigurationException("Path is required");

        configurationManager.AddVault(
            vaultUrl: options.Url,
            token: options.Token,
            path: options.Path,
            mountPoint: options.MountPoint,
            version: options.Version
        );

        return configurationManager;
    }

    private static VaultOptions GetOptions(IConfigurationManager configurationManager)
    {
        var section = configurationManager.GetSection(VaultSectionKey);

        return new VaultOptions
        {
            Url = section[UrlKey],
            Token = section[TokenKey],
            Path = section[PathKey],
            MountPoint = section[MountPointKey],
            Version = int.TryParse(section[VersionKey], out var version) ? version : null
        };
    }
}
