using Covenant.Integration.Tests.Configuration;
using Xunit;

[assembly:TestCollectionOrderer(DisplayNameOrderer.FullName,DisplayNameOrderer.Assembly)]
[assembly:CollectionBehavior(DisableTestParallelization = true)]

namespace Covenant.Integration.Tests.Configuration
{
    [TestCaseOrderer(PriorityOrderer.FullName, PriorityOrderer.Assembly)]
    public class BaseTestOrder
    {
    }
}