using System.Diagnostics.CodeAnalysis;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using HomeInventory.UI.ViewModels;
using HomeInventory.UI.Views;

namespace HomeInventory.UI;

[SuppressMessage("Performance", "CA1859:Use concrete types when possible for improved performance")]
public partial class App : Application
{
    private readonly ViewLocator _viewLocator = new AppViewLocator();
    private readonly Lazy<INavigationService> _navigationService;

    public App() => _navigationService = new(() => new NavigationService(ApplicationLifetime, _viewLocator));

    private INavigationService NavigationService => _navigationService.Value;
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        DataTemplates.Add(_viewLocator);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        switch (ApplicationLifetime)
        {
            case IClassicDesktopStyleApplicationLifetime desktop:
                desktop.MainWindow = new MainWindow();
                break;
        }

        NavigationService.NavigateTo(NavigationService.Main);
        base.OnFrameworkInitializationCompleted();
    }
}

public interface INavigationService
{
    BaseViewModel Main { get; }
    void NavigateTo<TViewModel>(TViewModel viewModel) where TViewModel : BaseViewModel;
}

public class NavigationService(IApplicationLifetime? lifetime, ViewLocator viewLocator) : INavigationService
{
    private readonly IApplicationLifetime? _lifetime = lifetime;
    private readonly ViewLocator _viewLocator = viewLocator;

    public BaseViewModel Main { get; } = new MainViewModel();

    public void NavigateTo<TViewModel>(TViewModel viewModel) where TViewModel : BaseViewModel
    {
        switch (_lifetime)
        {
            case IClassicDesktopStyleApplicationLifetime desktop:
                if (desktop.MainWindow != null)
                {
                    desktop.MainWindow.DataContext = viewModel;
                }

                break;
            case ISingleViewApplicationLifetime singleViewPlatform:
                singleViewPlatform.MainView = _viewLocator.Build(viewModel);
                break;
        }
    }
}