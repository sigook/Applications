namespace Covenant.Common.Configuration;

public class IdentityConfiguration
{
    public const string SectionName = "Identity";

    public string IssuerUri { get; set; }
    public string SigningCertificateName { get; set; }
    public string EncryptionCertificateName { get; set; }
    public string FunctionsClientId { get; set; }
    public string FunctionsClientSecret { get; set; }
    public string Microsoft365ClientId { get; set; }
    public string Microsoft365ClientSecret { get; set; }
}
