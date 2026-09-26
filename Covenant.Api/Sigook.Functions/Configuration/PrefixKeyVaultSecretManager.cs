using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration;

namespace Sigook.Functions.Configuration;

public class PrefixKeyVaultSecretManager(string prefix) : KeyVaultSecretManager
{
    private readonly string _prefix = $"{prefix}--";

    public override bool Load(SecretProperties properties) =>
        properties.Name.StartsWith(_prefix, StringComparison.OrdinalIgnoreCase);

    public override string GetKey(KeyVaultSecret secret) =>
        secret.Name[_prefix.Length..].Replace("--", ConfigurationPath.KeyDelimiter);
}
