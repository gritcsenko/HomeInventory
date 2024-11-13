using HomeInventory.Api;

using var log = SerilogConfigurator.CreateBootstrapLogger();

var builder = new AppBuilder(args);
await Execute.AndCatchAsync(
    async () =>
    {
        await using var app = builder.Build();
        await app.RunAsync();
    },
    (Exception ex) => log.Fatal(ex, "An unhandled exception occurred during bootstrapping"));

[System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1515:Consider making public types internal", Justification = "For testing")]
public partial class Program
{
    protected Program()
    {
    }
}
