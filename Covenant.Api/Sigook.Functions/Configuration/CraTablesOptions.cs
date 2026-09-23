namespace Sigook.Functions.Configuration;

public class CraTablesOptions
{
    public const string SectionName = "CraTables";
    public const string StorageConnectionName = "CraTablesStorage";
    public const string ContainerName = "cra-tables";

    public string CppApiUrl { get; set; } = string.Empty;

    public string TaxApiUrl { get; set; } = string.Empty;
}
