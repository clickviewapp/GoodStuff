namespace ClickView.GoodStuff.Configuration.Vault;

using Microsoft.Extensions.Configuration;

internal class VaultTokenFileConfigurationProvider : ConfigurationProvider
{
    public override void Load()
    {
        var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".vault-token");

        if (!File.Exists(path))
            return;

        var token = File.ReadAllText(path).Trim();

        if (!string.IsNullOrEmpty(token))
            Data["Vault:Token"] = token;
    }
}
