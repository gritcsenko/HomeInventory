using HomeInventory.Api;
using HomeInventory.Core;

using var log = SerilogConfigurator.CreateBootstrapLogger();

await Execute.AndCatchAsync(
    async () => await new AppBuilder(args).Build().RunAsync(),
    (Exception ex) => log.Fatal(ex, "An unhandled exception occurred during bootstrapping"));

public partial class Program
{
    protected Program()
    {
    }
}
