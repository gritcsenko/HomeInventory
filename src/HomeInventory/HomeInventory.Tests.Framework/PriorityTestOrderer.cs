using HomeInventory.Tests.Framework.Attributes;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace HomeInventory.Tests.Framework;

internal class PriorityTestOrderer(IMessageSink sink) : ITestCaseOrderer
{
    public IMessageSink Sink { get; } = sink;

    public IEnumerable<TTestCase> OrderTestCases<TTestCase>(IEnumerable<TTestCase> testCases)
        where TTestCase : ITestCase
    {
        var queue = new PriorityQueue<TTestCase, int>();
        foreach (var testCase in testCases)
        {
            queue.Enqueue(testCase, GetPriority(testCase));
        }

        while (queue.Count > 0)
        {
            yield return queue.Dequeue();
        }
    }

    private static int GetPriority<TTestCase>(TTestCase testCase)
        where TTestCase : ITestCase
    {
        const int DefaultPriority = 0;
        var attribute = testCase.TestMethod.Method.GetCustomAttributes(typeof(TestPriorityAttribute)).FirstOrDefault();
        return attribute?.GetNamedArgument<int>(nameof(TestPriorityAttribute.Priority)) ?? DefaultPriority;
    }
}
