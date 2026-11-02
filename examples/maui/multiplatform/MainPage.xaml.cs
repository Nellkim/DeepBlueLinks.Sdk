namespace DeepBlueLinksExample;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        BindingContext = LinkResults.Current;
    }

    async void OnCheckClicked(object? sender, EventArgs e)
    {
        CheckButton.IsEnabled = false;
        await App.CheckForDeferredLinkAsync();
        CheckButton.IsEnabled = true;
    }
}
