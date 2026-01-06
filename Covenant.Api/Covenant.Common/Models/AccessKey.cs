namespace Covenant.Common.Models
{
    public class AccessKey
    {
        public AccessKey(string containerName, string connectionString)
        {
            ContainerName = containerName;
            ConnectionString = connectionString;
        }

        public string ContainerName { get; set; }
        public string ConnectionString { get; set; }
    }
}
