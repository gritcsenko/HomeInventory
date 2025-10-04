namespace HomeInventory.Tests.Acceptance.Support;

internal static class ScenarioContextExtensions
{
    public static async ValueTask SetAllAsync<T>(this ScenarioContext context, IAsyncEnumerable<T> source, string key) =>
        context.Set(await source.ToArrayAsync(), key);

    public static void SetAll<T>(this ScenarioContext context, IEnumerable<T> source, string key) =>
        context.Set(source.ToArray(), key);

    public static IEnumerable<T> GetAll<T>(this ScenarioContext context, string key) =>
        context.Get<T[]>(key);
}
