using Microsoft.Maui.LifecycleEvents;

namespace DeepBlueLinksExample;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.ConfigureLifecycleEvents(lifecycle =>
        {
#if IOS || MACCATALYST
            lifecycle.AddiOS(ios =>
            {
                ios.FinishedLaunching((app, data) => HandleUserActivity(app.UserActivity));
                ios.ContinueUserActivity((app, userActivity, handler) => HandleUserActivity(userActivity));
                // iOS delivers custom URL scheme links as URLs, not user activities: OpenUrl, and with scenes
                // the second SceneWillConnect and SceneOpenUrl, forward them. UrlContexts is null without a URL.
                ios.OpenUrl((app, url, options) => HandleUrl(url));
                if (OperatingSystem.IsIOSVersionAtLeast(13))
                {
                    ios.SceneWillConnect((scene, session, options) => HandleUserActivity(options.UserActivities.ToArray()
                        .FirstOrDefault(a => a.ActivityType == Foundation.NSUserActivityType.BrowsingWeb)));
                    ios.SceneContinueUserActivity((scene, userActivity) => HandleUserActivity(userActivity));
                    ios.SceneWillConnect((scene, session, options) => HandleUrl(options.UrlContexts?.ToArray().FirstOrDefault()?.Url));
                    ios.SceneOpenUrl((scene, urlContexts) => HandleUrl(urlContexts?.ToArray().FirstOrDefault()?.Url));
                }
            });
#endif
#if ANDROID
            lifecycle.AddAndroid(android =>
            {
                android.OnCreate((activity, bundle) => HandleIntent(activity.Intent));
                android.OnNewIntent((activity, intent) => HandleIntent(intent));
            });
#endif
        });

        return builder.Build();
    }

#if IOS || MACCATALYST
    static bool HandleUserActivity(Foundation.NSUserActivity? activity)
    {
        if (activity?.ActivityType != Foundation.NSUserActivityType.BrowsingWeb || activity.WebPageUrl is null)
            return false;
        SendAppLink(activity.WebPageUrl.ToString()!);
        return true;
    }

    static bool HandleUrl(Foundation.NSUrl? url)
    {
        if (url?.AbsoluteString is null)
            return false;
        SendAppLink(url.AbsoluteString);
        return true;
    }
#endif

#if ANDROID
    static void HandleIntent(Android.Content.Intent? intent)
    {
        if (intent?.Action == Android.Content.Intent.ActionView && intent.Data is not null)
            SendAppLink(intent.Data.ToString()!);
    }
#endif

    static void SendAppLink(string url)
    {
        if (Uri.TryCreate(url, UriKind.Absolute, out var uri))
            Application.Current?.SendOnAppLinkRequestReceived(uri);
    }
}
