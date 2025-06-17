using System.Diagnostics.CodeAnalysis;
using Avalonia;
using Avalonia.ReactiveUI;

namespace HomeInventory.UI.Desktop;

[SuppressMessage("Major Code Smell", "S1118:Utility classes should not have public constructors")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    // Avalonia configuration, don't remove; also used by visual designer.
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .UseReactiveUI()
            .LogToTrace();
}