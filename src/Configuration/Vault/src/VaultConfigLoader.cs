namespace ClickView.GoodStuff.Configuration.Vault;

using System.Text.Json;
using Microsoft.Extensions.Configuration;
using VaultSharp;

public class VaultConfigLoader(IVaultClient vaultClient, string path, string? mountPoint, int? version)
{
    public async Task<IDictionary<string, string?>> LoadSecretsAsync()
    {
        var secret = await vaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync(path: path, mountPoint: mountPoint, version: version) ??
                     throw new VaultConfigurationException(
                         $"Failed to read secrets for Path '{path}' at MountPoint '{mountPoint}'");

        // The items dictionary needs to be case insensitive
        var result = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);

        // Populate the result dictionary
        foreach (var secretData in secret.Data.Data)
        {
            if (secretData.Value is not JsonElement jsonElement)
                throw new VaultConfigurationException("Value is not JsonElement");

            Populate(result, secretData.Key, jsonElement);
        }

        return result;
    }

    private static void Populate(Dictionary<string, string?> dictionary, string key, JsonElement jsonElement)
    {
        // For array values, add configuration items formatted like `key:{index}`
        if (jsonElement.ValueKind is JsonValueKind.Array)
        {
            var counter = 0;
            foreach (var item in jsonElement.EnumerateArray())
            {
                Populate(dictionary, key + ConfigurationPath.KeyDelimiter + counter, item);
                ++counter;
            }
        }
        // For object values, flatten them out
        else if (jsonElement.ValueKind is JsonValueKind.Object)
        {
            foreach (var item in jsonElement.EnumerateObject())
            {
                Populate(dictionary, key + ConfigurationPath.KeyDelimiter + item.Name, item.Value);
            }
        }
        // All other values should be key value pairs
        else
        {
            dictionary[FormatKey(key)] = GetValue(jsonElement);
        }
    }

    private static string FormatKey(string key)
    {
        // Remove any __ characters in the key and replace them with the path delimiter.
        // This isn't really needed, but does allow storing keys in the same format environment variables would be stored.
        return key.Replace("__", ConfigurationPath.KeyDelimiter);
    }

    private static string? GetValue(JsonElement jsonElement)
    {
        return jsonElement.ValueKind switch
        {
            JsonValueKind.Undefined => throw new NotImplementedException("Undefined JSON values are not supported"),
            JsonValueKind.Object => throw new NotImplementedException("Object JSON values are not supported"),
            JsonValueKind.Array => throw new NotImplementedException("Array JSON values is not supported"),
            JsonValueKind.String => jsonElement.GetString(),
            JsonValueKind.Number => jsonElement.ToString(),
            JsonValueKind.True => "true",
            JsonValueKind.False => "false",
            JsonValueKind.Null => null,
            _ => throw new NotImplementedException($"Unknown JsonValueKind '{jsonElement.ValueKind}'")
        };
    }
}
