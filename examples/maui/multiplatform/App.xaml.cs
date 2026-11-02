namespace DeepBlueLinksExample;

public partial class App : Application
{
    const string DeferredLinkCheckedKey = "deferred_link_checked";

    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new NavigationPage(new MainPage()));
    }

    protected override async void OnStart()
    {
        base.OnStart();
        if (Preferences.Default.Get(DeferredLinkCheckedKey, false))
        {
            LinkResults.Current.DeferredLink = "Checked on the first launch";
            return;
        }
        Preferences.Default.Set(DeferredLinkCheckedKey, true);
        await CheckForDeferredLinkAsync();
    }

    public static async Task CheckForDeferredLinkAsync()
    {
        var results = LinkResults.Current;
        results.DeferredLink = "Checking…";
        try
        {
            var link = await DeepBlueLinks.FindInstallLinkAsync();
            results.DeferredLink = link is null ? "No deferred link" : LinkResults.Describe(link);
        }
        catch (Exception ex)
        {
            results.DeferredLink = ex.Message;
            results.ShowError("Finding the install link", ex);
        }
    }

    protected override async void OnAppLinkRequestReceived(Uri uri)
    {
        base.OnAppLinkRequestReceived(uri);
        if (uri.Scheme == DeepBlueLinksConfig.UrlScheme)
        {
            LinkResults.Current.ShowOpenedLink(uri, null, "/" + uri.Host + uri.AbsolutePath, uri.Query);
            return;
        }
        try
        {
            var link = await DeepBlueLinks.ResolveAsync(uri.ToString());
            var destination = link is null ? uri : new Uri(link.LongUrl);
            LinkResults.Current.ShowOpenedLink(uri, link, destination.AbsolutePath, destination.Query);
        }
        catch (Exception ex)
        {
            LinkResults.Current.ShowError($"Resolving {uri}", ex);
        }
    }
}
