namespace HomeInventory.UI.Presentation;

public sealed partial class UIShell : UserControl, IContentControlProvider
{
    public UIShell()
    {
        this.InitializeComponent();
    }
    public ContentControl ContentControl => Splash;
}
