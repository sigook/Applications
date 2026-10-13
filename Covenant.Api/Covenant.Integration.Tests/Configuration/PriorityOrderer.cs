using Xunit.Abstractions;
using Xunit.Sdk;
using Xunit;

namespace Covenant.Integration.Tests.Configuration
{
    public class DisplayNameOrderer : ITestCollectionOrderer
    {
        public const string FullName = "Covenant.Integration.Tests.Configuration.DisplayNameOrderer";
        public const string Assembly = "Covenant.Integration.Tests";
        public IEnumerable<ITestCollection> OrderTestCollections(IEnumerable<ITestCollection> testCollections)
        {
            return testCollections.OrderBy(collection => collection.DisplayName);
        }
    }

    public class PriorityOrderer : ITestCaseOrderer
    {
        public const string FullName = "Covenant.Integration.Tests.Configuration.PriorityOrderer";
        public const string Assembly = "Covenant.Integration.Tests";

        public IEnumerable<TTestCase> OrderTestCases<TTestCase>(IEnumerable<TTestCase> testCases) where TTestCase : ITestCase =>
            testCases.OrderBy(testCase => testCase.TestMethod.Method.Name, StringComparer.OrdinalIgnoreCase);
    }
}
