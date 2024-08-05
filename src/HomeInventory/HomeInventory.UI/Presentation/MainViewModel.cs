namespace HomeInventory.UI.Presentation;

public partial class MainViewModel : ObservableObject
{
    private IAuthenticationService _authentication;

    private INavigator _navigator;

    [ObservableProperty]
    private string? name;

    private int count = 1;

    [ObservableProperty]
    private string counterText = "Press Me";

    public MainViewModel(
        IStringLocalizer localizer,
        IOptions<AppConfig> appInfo,
        IAuthenticationService authentication,
        INavigator navigator)
    {
        _navigator = navigator;
        _authentication = authentication;
        Title = "Main";
        Title += $" - {localizer["ApplicationName"]}";
        Title += $" - {appInfo?.Value?.Environment}";
        GoToSecond = new AsyncRelayCommand(GoToSecondView);
        Counter = new RelayCommand(OnCount);
        Logout = new AsyncRelayCommand(DoLogout);
    }
    public string? Title { get; }

    public ICommand GoToSecond { get; }

    public ICommand Counter { get; }

    public ICommand Logout { get; }

    private async Task GoToSecondView()
    {
        await _navigator.NavigateViewModelAsync<SecondViewModel>(this, data: new Entity(Name!));
    }


    private void OnCount()
    {
        CounterText = ++count switch
        {
            1 => "Pressed Once!",
            _ => $"Pressed {count} times!"
        };
    }
    public async Task DoLogout(CancellationToken token)
    {
        await _authentication.LogoutAsync(token);
    }
}
