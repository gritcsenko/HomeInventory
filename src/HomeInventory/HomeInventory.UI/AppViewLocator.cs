using HomeInventory.UI.ViewModels;
using HomeInventory.UI.Views;

namespace HomeInventory.UI;

public sealed class AppViewLocator : ViewLocator
{
    public AppViewLocator() 
        : base(new())
    {
        Register<MainViewModel, MainView>();
    }
}