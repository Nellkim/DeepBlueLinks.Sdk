using Android.Content;
using Android.Content.PM;

namespace DeepBlueLinksExample;

[Activity(MainLauncher = true, Exported = true, LaunchMode = LaunchMode.SingleTask)]
[IntentFilter(
    new[] { Intent.ActionView },
    Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
    DataScheme = "https",
    DataHost = DeepBlueLinksConfig.LinkDomain,
    AutoVerify = true)]
[IntentFilter(
    new[] { Intent.ActionView },
    Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
    DataScheme = DeepBlueLinksConfig.UrlScheme)]
public class MainActivity : Activity
{
    const string PreferencesName = "deep_blue_links";
    const string DeferredLinkCheckedKey = "deferred_link_checked";

    TextView openedLink = null!;
    TextView route = null!;
    TextView deferredLink = null!;
    TextView lastError = null!;
    Button checkButton = null!;

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        SetContentView(Resource.Layout.activity_main);
        SetTitle(Resource.String.screen_title);
        openedLink = FindViewById<TextView>(Resource.Id.opened_link)!;
        route = FindViewById<TextView>(Resource.Id.route)!;
        deferredLink = FindViewById<TextView>(Resource.Id.deferred_link)!;
        lastError = FindViewById<TextView>(Resource.Id.last_error)!;
        checkButton = FindViewById<Button>(Resource.Id.check_deferred_link)!;
        checkButton.Click += (sender, e) => CheckForDeferredLink();

        HandleIntent(Intent);

        var preferences = GetSharedPreferences(PreferencesName, FileCreationMode.Private)!;
        if (preferences.GetBoolean(DeferredLinkCheckedKey, false))
        {
            deferredLink.Text = "Checked on the first launch";
            return;
        }
        preferences.Edit()!.PutBoolean(DeferredLinkCheckedKey, true)!.Apply();
        CheckForDeferredLink();
    }

    protected override void OnNewIntent(Intent? intent)
    {
        base.OnNewIntent(intent);
        Intent = intent;
        HandleIntent(intent);
    }

    void HandleIntent(Intent? intent)
    {
        if (intent?.Action == Intent.ActionView && Uri.TryCreate(intent.Data?.ToString(), UriKind.Absolute, out var uri))
            OnLinkReceived(uri);
    }

    async void OnLinkReceived(Uri uri)
    {
        if (uri.Scheme == DeepBlueLinksConfig.UrlScheme)
        {
            ShowOpenedLink(uri, "URL scheme: routed without resolve");
            ShowRoute("/" + uri.Host + uri.AbsolutePath, uri.Query);
            return;
        }
        ShowOpenedLink(uri, "Resolving…");
        ResolvedLink? link = null;
        try
        {
            link = await DeepBlueLinks.ResolveAsync(uri.ToString());
            ShowOpenedLink(uri, link is null ? "Not one of the app's short links (resolve answered 404): routed as it is" : Describe(link));
        }
        catch (Exception ex)
        {
            ShowOpenedLink(uri, "Resolve failed: routed as it is");
            ShowError($"Resolving {uri}", ex);
        }
        var destination = link is null ? uri : new Uri(link.LongUrl);
        ShowRoute(destination.AbsolutePath, destination.Query);
    }

    async void CheckForDeferredLink()
    {
        checkButton.Enabled = false;
        deferredLink.Text = "Checking…";
        try
        {
            var link = await DeepBlueLinks.FindInstallLinkAsync();
            deferredLink.Text = link is null ? "No deferred link" : Describe(link);
            if (link is not null)
            {
                var destination = new Uri(link.LongUrl);
                ShowRoute(destination.AbsolutePath, destination.Query);
            }
        }
        catch (Exception ex)
        {
            deferredLink.Text = ex.Message;
            ShowError("Finding the install link", ex);
        }
        checkButton.Enabled = true;
    }

    void ShowOpenedLink(Uri uri, string outcome) => openedLink.Text = $"{uri}\n{outcome}";

    void ShowRoute(string path, string query) => route.Text = path + query;

    void ShowError(string action, Exception exception) => lastError.Text = $"{action} failed: {exception.Message}";

    static string Describe(ResolvedLink link) =>
        $"id: {link.Id}\nlongUrl: {link.LongUrl}\nshortUrl: {link.ShortUrl}\nisSandbox: {(link.IsSandbox is true ? "true" : "absent (a live link)")}";
}
