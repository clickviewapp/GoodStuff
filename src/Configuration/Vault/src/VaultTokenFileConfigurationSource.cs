namespace ClickView.GoodStuff.Configuration.Vault;

using Microsoft.Extensions.Configuration;

internal class VaultTokenFileConfigurationSource : IConfigurationSource
{
    public IConfigurationProvider Build(IConfigurationBuilder builder) => new VaultTokenFileConfigurationProvider();
}
