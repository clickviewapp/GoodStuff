namespace ClickView.GoodStuff.Configuration.Vault;

using Microsoft.Extensions.Configuration;

public class VaultConfigurationProvider(VaultConfigLoader configLoader) : ConfigurationProvider
{
    public override void Load()
    {
        // Set the Data for the configuration provider to the config returned from our loader
        Data = configLoader.LoadSecretsAsync().GetAwaiter().GetResult();
    }
}
