using Covenant.Test.Utils.Configuration;
using Xunit;

[assembly:TestCollectionOrderer(DisplayNameOrderer.FullName,DisplayNameOrderer.Assembly)]
[assembly:CollectionBehavior(DisableTestParallelization = true)]

namespace Covenant.Test.Utils.Configuration
{
    [TestCaseOrderer(PriorityOrderer.FullName, PriorityOrderer.Assembly)]
    public class BaseTestOrder
    {
    }
}