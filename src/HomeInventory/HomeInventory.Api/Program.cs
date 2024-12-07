using System.Diagnostics.CodeAnalysis;

namespace HomeInventory.Api;

[SuppressMessage("Design", "CA1052:Static holder types should be Static or NotInheritable", Justification = "By design")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class Program
{
    protected Program()
    {
    }

    public static async Task Main(string[] args)
    {
        await using var log = LoggingModule.CreateBootstrapLogger();

        var builder = new AppBuilder(args);
        await Execute.AndCatchAsync(
            async () =>
            {
                await using var app = await builder.BuildAsync();
                await app.RunAsync();
            },
            (Exception ex) => log.Fatal(ex, "An unhandled exception occurred during bootstrapping"));
    }
}
