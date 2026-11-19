using Xunit.Abstractions;
using Xunit.Sdk;

namespace KeelteKoolV2.SeleniumTesting
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class TestPriorityAttribute : Attribute
    {
        public int Priority { get; }

        public TestPriorityAttribute(int priority)
        {
            Priority = priority;
        }
    }

    // Kasutus: [TestCaseOrderer("KeelteKoolV2.SeleniumTesting.PriorityOrderer", "KeelteKoolV2.SeleniumTesting")] klassil
    // ja [TestPriority(n)] meetodil. Väiksem number jookseb enne, märgistamata testid jooksevad viimasena.
    public class PriorityOrderer : ITestCaseOrderer
    {
        public IEnumerable<TTestCase> OrderTestCases<TTestCase>(IEnumerable<TTestCase> testCases)
            where TTestCase : ITestCase
        {
            return testCases
                .OrderBy(GetPriority)
                .ThenBy(tc => tc.TestMethod.Method.Name);
        }

        private static int GetPriority<TTestCase>(TTestCase testCase) where TTestCase : ITestCase
        {
            IAttributeInfo? attribute = testCase.TestMethod.Method
                .GetCustomAttributes(typeof(TestPriorityAttribute).AssemblyQualifiedName)
                .FirstOrDefault();

            return attribute is null ? int.MaxValue : Convert.ToInt32(attribute.GetConstructorArguments().First());
        }
    }
}
