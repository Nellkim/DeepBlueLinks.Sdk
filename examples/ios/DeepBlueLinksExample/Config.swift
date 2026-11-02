// Every value to change for your app. The link domain and the URL scheme also appear in
// DeepBlueLinksExample.entitlements and Info.plist, and the bundle ID in the target's build settings.
enum Config {
    static let apiURL = "https://thedbl.ink"

    // The app's sandbox key, which starts with sandbox_, in debug builds, and its live key in release builds.
    static let apiKey = "YOUR_API_KEY"

    static let linkDomain = "demo.thedbl.ink"

    static let urlScheme = "dblexample"
}
