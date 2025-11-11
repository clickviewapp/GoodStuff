namespace ClickView.GoodStuff.Configuration.Vault;

using Microsoft.Extensions.Configuration;
using VaultSharp;
using VaultSharp.V1.AuthMethods.Token;

public static class ConfigurationBuilderExtensions
{
    public static IConfigurationBuilder AddVault(
        this IConfigurationBuilder configurationBuilder,
        string vaultUrl,
        string token,
        string path,
        string? mountPoint = null,
        int? version = null)
    {
        ArgumentNullException.ThrowIfNull(configurationBuilder);
        ArgumentNullException.ThrowIfNull(vaultUrl);
        ArgumentNullException.ThrowIfNull(token);
        ArgumentNullException.ThrowIfNull(path);

        var client = new VaultClient(new VaultClientSettings(vaultUrl, new TokenAuthMethodInfo(token)));

        return AddVault(configurationBuilder, client, path, mountPoint, version);
    }

    public static IConfigurationBuilder AddVault(
        this IConfigurationBuilder configurationBuilder,
        IVaultClient vaultClient,
        string path,
        string? mountPoint = null,
        int? version = null)
    {
        ArgumentNullException.ThrowIfNull(configurationBuilder);
        ArgumentNullException.ThrowIfNull(vaultClient);
        ArgumentNullException.ThrowIfNull(path);

        var configLoader = new VaultConfigLoader(vaultClient, path: path, mountPoint: mountPoint, version: version);
        configurationBuilder.Add(new VaultConfigurationSource(configLoader));

        return configurationBuilder;
    }
}
