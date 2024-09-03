namespace HomeInventory.Api;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1052:Static holder types should be Static or NotInheritable", Justification = "By design")]
public partial class Program
{
    protected Program()
    {
    }

    public static async Task Main(string[] args)
    {
        using var log = LoggingModule.CreateBootstrapLogger();

        var builder = new AppBuilder(args);
        await Execute.AndCatchAsync(
            async () =>
            {
                await using var app = builder.Build();
                await app.RunAsync();
            },
            (Exception ex) => log.Fatal(ex, "An unhandled exception occurred during bootstrapping"));
    }
}
