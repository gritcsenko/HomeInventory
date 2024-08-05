namespace HomeInventory.UI.Presentation;

public partial class LoginViewModel : ObservableObject
{
    private readonly IAuthenticationService _authentication;

    private readonly INavigator _navigator;

    private readonly IDispatcher _dispatcher;


    public LoginViewModel(
        IDispatcher dispatcher,
        INavigator navigator,
        IAuthenticationService authentication)
    {
        _dispatcher = dispatcher;
        _navigator = navigator;
        _authentication = authentication;
        Login = new AsyncRelayCommand(DoLogin);
    }

    private async Task DoLogin()
    {
        var success = await _authentication.LoginAsync(_dispatcher);
        if (success)
        {
            await _navigator.NavigateViewModelAsync<MainViewModel>(this, qualifier: Qualifiers.ClearBackStack);
        }
    }

    public string Title { get; } = "Login";

    public ICommand Login { get; }
}
