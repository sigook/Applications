using Covenant.Common.Models;

namespace Covenant.Common.Configuration
{
    public class AzureStorageConfiguration
    {
        public AzureStorageConfiguration()
        {
        }

        public AzureStorageConfiguration(IEnumerable<AccessKey> keys, AccessKey defaultAccess = null)
        {
            AccessKeys = new List<AccessKey>(keys);
            DefaultAccessKey = defaultAccess;
        }

        public IReadOnlyCollection<AccessKey> AccessKeys { get; }
        public AccessKey DefaultAccessKey { get; }
        
    }
}