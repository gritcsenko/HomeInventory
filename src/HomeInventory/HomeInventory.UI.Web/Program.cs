using System.Diagnostics.CodeAnalysis;
using HomeInventory.UI;
using Avalonia;
using Avalonia.Browser;
using Avalonia.ReactiveUI;
using AvaloniaInside.Shell;

[SuppressMessage("Major Code Smell", "S1118:Utility classes should not have public constructors")]
[SuppressMessage("Major Bug", "S3903:Types should be defined in named namespaces")]
internal sealed partial class Program
{
    private static Task Main(string[] args) => BuildAvaloniaApp()
        .WithInterFont()
        .UseReactiveUI()
        .UseShell()
        .StartBrowserAppAsync("out");

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>();
}