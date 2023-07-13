using HomeInventory.Api;
using HomeInventory.Core;

using var log = SerilogConfigurator.CreateBootstrapLogger();

var builder = new AppBuilder(args);
await Execute.AndCatchAsync(
    async () =>
    {
        await using var app = builder.Build();
        await app.RunAsync();
    },
    (Exception ex) => log.Fatal(ex, "An unhandled exception occurred during bootstrapping"));

public partial class Program
{
    protected Program()
    {
    }
}
