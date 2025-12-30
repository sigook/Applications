using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Covenant.Test.Utils.Configuration
{
    public class DisplayNameOrderer : ITestCollectionOrderer
    {
        public const string FullName = "Covenant.Test.Utils.Configuration.DisplayNameOrderer";
        public const string Assembly = "Covenant.Test.Utils";
        public IEnumerable<ITestCollection> OrderTestCollections(IEnumerable<ITestCollection> testCollections)
        {
            return testCollections.OrderBy(collection => collection.DisplayName);
        }
    }



    public class PriorityOrderer : ITestCaseOrderer
    {
        public const string FullName = "Covenant.Test.Utils.Configuration.PriorityOrderer";
        public const string Assembly = "Covenant.Test.Utils";

        public IEnumerable<TTestCase> OrderTestCases<TTestCase>(IEnumerable<TTestCase> testCases) where TTestCase : ITestCase
        {
            var sortedMethods = new SortedDictionary<int, List<TTestCase>>();
            foreach (TTestCase testCase in testCases)
            {
                int priority = 0;
                foreach (IAttributeInfo attr in testCase.TestMethod.Method.GetCustomAttributes(typeof(TestOrderAttribute).AssemblyQualifiedName))
                {
                    priority = attr.GetNamedArgument<int>("Priority");
                }
                GetOrCreate(sortedMethods, priority).Add(testCase);
            }

            foreach (var list in sortedMethods.Keys.Select(priority => sortedMethods[priority]))
            {
                list.Sort((x, y) => StringComparer.OrdinalIgnoreCase.Compare(x.TestMethod.Method.Name, y.TestMethod.Method.Name));
                foreach (TTestCase testCase in list) yield return testCase;
            }
        }

        private static TValue GetOrCreate<TKey, TValue>(IDictionary<TKey, TValue> dictionary, TKey key) where TValue : new()
        {
            if (dictionary.TryGetValue(key, out TValue result)) return result;
            result = new TValue();
            dictionary[key] = result;
            return result;
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class TestOrderAttribute : Attribute
    {
        public int Priority { get; private set; }
        public TestOrderAttribute(int priority)
        {
            Priority = priority;
        }
    }
}