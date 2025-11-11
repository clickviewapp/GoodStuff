namespace ClickView.GoodStuff.Configuration.Vault;

using Microsoft.Extensions.Configuration;

public class VaultConfigurationSource(VaultConfigLoader configLoader) : IConfigurationSource
{
    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new VaultConfigurationProvider(configLoader);
    }
}
