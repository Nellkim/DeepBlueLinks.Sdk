namespace DeepBlueLinksExample;

// The values to change for your app. The README lists every file that repeats them,
// including the bundle ID (com.example.deepbluelinks) in Info.plist.
public static class DeepBlueLinksConfig
{
    public const string ApiUrl = "https://thedbl.ink";

    // The sandbox key, which starts with sandbox_, in debug builds; the live key in release builds.
    public const string ApiKey = "YOUR_API_KEY";

    // Also in Entitlements.plist.
    public const string LinkDomain = "demo.thedbl.ink";

    // Also in Info.plist.
    public const string UrlScheme = "dblexample";
}
