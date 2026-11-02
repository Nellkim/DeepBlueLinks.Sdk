namespace DeepBlueLinksExample;

[Register("SceneDelegate")]
public class SceneDelegate : UIResponder, IUIWindowSceneDelegate
{
    const string DeferredLinkCheckedKey = "deferred_link_checked";

    readonly MainViewController mainViewController = new();

    [Export("window")]
    public UIWindow? Window { get; set; }

    [Export("scene:willConnectToSession:options:")]
    public void WillConnect(UIScene scene, UISceneSession session, UISceneConnectionOptions connectionOptions)
    {
        if (scene is not UIWindowScene windowScene)
            return;
        Window ??= new UIWindow(windowScene);
        Window.RootViewController = new UINavigationController(mainViewController);
        Window.MakeKeyAndVisible();

        // A link that launches the app arrives here, not in ContinueUserActivity or OpenUrlContexts.
        // UrlContexts is null when the app opens without a URL.
        HandleUserActivity(connectionOptions.UserActivities.ToArray().FirstOrDefault(a => a.ActivityType == NSUserActivityType.BrowsingWeb));
        HandleUrl(connectionOptions.UrlContexts?.ToArray().FirstOrDefault()?.Url);
        CheckForDeferredLinkOnFirstLaunch();
    }

    [Export("scene:continueUserActivity:")]
    public void ContinueUserActivity(UIScene scene, NSUserActivity userActivity) => HandleUserActivity(userActivity);

    [Export("scene:openURLContexts:")]
    public void OpenUrlContexts(UIScene scene, NSSet<UIOpenUrlContext> urlContexts) =>
        HandleUrl(urlContexts.ToArray().FirstOrDefault()?.Url);

    void HandleUserActivity(NSUserActivity? activity)
    {
        if (activity?.ActivityType == NSUserActivityType.BrowsingWeb)
            HandleUrl(activity.WebPageUrl);
    }

    void HandleUrl(NSUrl? url)
    {
        if (Uri.TryCreate(url?.AbsoluteString, UriKind.Absolute, out var uri))
            _ = mainViewController.OpenLinkAsync(uri);
    }

    void CheckForDeferredLinkOnFirstLaunch()
    {
        var defaults = NSUserDefaults.StandardUserDefaults;
        if (defaults.BoolForKey(DeferredLinkCheckedKey))
        {
            mainViewController.ShowDeferredLink("Checked on the first launch");
            return;
        }
        defaults.SetBool(true, DeferredLinkCheckedKey);
        _ = mainViewController.CheckForDeferredLinkAsync();
    }
}
