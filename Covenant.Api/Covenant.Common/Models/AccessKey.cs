namespace Covenant.Common.Models;

public class AccessKey(string containerName, string connectionString)
{
    public string ContainerName { get; set; } = containerName;
    public string ConnectionString { get; set; } = connectionString;
}
