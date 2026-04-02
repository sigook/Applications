using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration;

namespace Covenant.IdentityServer.Configuration;

public class PrefixKeyVaultSecretManager : KeyVaultSecretManager
{
    private readonly string _prefix;

    public PrefixKeyVaultSecretManager(string prefix)
    {
        _prefix = $"{prefix}--";
    }

    public override bool Load(SecretProperties properties)
    {
        return properties.Name.StartsWith(_prefix, StringComparison.OrdinalIgnoreCase);
    }

    public override string GetKey(KeyVaultSecret secret)
    {
        return secret.Name[_prefix.Length..].Replace("--", ConfigurationPath.KeyDelimiter);
    }
}
