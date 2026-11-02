# Deep Blue Links .NET for iOS example

A native .NET for iOS app, in C# with UIKit and without .NET MAUI, that does what the [.NET MAUI guide](https://deepbluelinks.io/docs/guides/maui) teaches: it's the guide's code in a native .NET for iOS app. The app opens from its short links as universal links and from its custom URL scheme. iOS hands each link to `SceneDelegate`: a universal link as a browsing-web user activity, in `WillConnect` at launch and in `ContinueUserActivity` while the app runs, and a URL scheme link as a URL, in `WillConnect`'s URL contexts or in `OpenUrlContexts`. The app resolves a universal link with `POST /api/links/resolve`, and routes by the URL itself when the link isn't one of its short links or the call fails. A URL scheme link is routed by its own path, without being resolved. On its first launch, the app calls `POST /api/links/check-deferred` with the timezone, the screen in pixels and the language. Its one screen shows the link that opened the app, the route, the deferred-link result and the last error.

## Requirements

Built with:

- .NET SDK 10.0.403
- The `ios` workload from workload set 10.0.401.1 (.NET for iOS 27.0.10722)
- Xcode 27.0

The app runs on iOS 15 or later and uses no NuGet packages. Universal links need an iPhone or iPad and an Apple developer account.

## Configure

1. Create the app in the [Deep Blue Links dashboard](https://app.deepbluelinks.io):
   - **iOS**: your Apple team ID and the bundle ID.
   - **URL scheme**: `dblexample`, or your own.
   - **API keys**: generate the sandbox key.
2. Replace the placeholders. Replace `demo` with your app's name in the Deep Blue Links dashboard.

   | Value | Placeholder | Where |
   |---|---|---|
   | API URL | `https://thedbl.ink` | `DeepBlueLinksConfig.cs` (`ApiUrl`) |
   | API key | `YOUR_API_KEY` | `DeepBlueLinksConfig.cs` (`ApiKey`) |
   | Link domain | `demo.thedbl.ink` | `DeepBlueLinksConfig.cs` (`LinkDomain`, which the screen shows) and `Entitlements.plist` (`applinks:demo.thedbl.ink`) |
   | URL scheme | `dblexample` | `DeepBlueLinksConfig.cs` (`UrlScheme`) and `Info.plist` (`CFBundleURLSchemes`) |
   | Bundle ID | `com.example.deepbluelinks` | `Info.plist` (`CFBundleIdentifier`) |

   Use the app's sandbox key, which starts with `sandbox_`, in debug builds, and the live key in release builds. See [sandbox links](https://deepbluelinks.io/docs/sandbox).
3. For each [custom domain](https://deepbluelinks.io/docs/custom-domains), add an `applinks:` entry to `Entitlements.plist`.
4. To run on a device, register an App ID with the bundle ID in your Apple developer account, enable its **Associated Domains** capability, and download a development provisioning profile for it that includes the device. The build picks the signing identity and the profile that match the bundle ID. To choose them yourself, add `-p:CodesignKey="Apple Development: <name>"` and `-p:CodesignProvision="<profile name>"` to the build.

## Run

```bash
dotnet build -t:Run -f net10.0-ios -p:RuntimeIdentifier=ios-arm64 -p:_DeviceName=<device UDID>
```

For the iOS simulator, use `-p:RuntimeIdentifier=iossimulator-arm64 -p:_DeviceName=:v2:udid=<simulator UDID>`. The simulator needs no provisioning profile, but the associated-domains entitlement makes the build ask for one, so add `-p:CodesignRequireProvisioningProfile=false`.

## Test it

- Universal links work on a device, not the simulator. Paste a short link into Notes and tap it; long-press it to choose between the app and Safari. The screen shows the resolved link's `id`, `longUrl`, `shortUrl` and `isSandbox`, and the route taken from `longUrl`.
- Typing the link into Safari's address bar opens the page, not the app, by design.
- In **Settings › Developer › Universal Links**, **Diagnostics** checks a URL against your installed app.
- iOS fetches `apple-app-site-association` through Apple's CDN, which can lag behind a change to your app's settings. While testing, `applinks:demo.thedbl.ink?mode=developer` with **Associated Domains Development** turned on fetches it directly.
- Open the URL scheme with `xcrun simctl openurl booted "dblexample://products/1234"`, and tap **Open** when iOS asks. The app shows the route, `/products/1234`, without calling the API, as the guide routes scheme URLs.
- To test a deferred link, delete the app and open a short link's preview page in Safari on the device. Then install and launch the app within 60 minutes, on the same network. "No deferred link" is normal on most launches. **Check for a deferred link** runs the lookup again. It's a testing aid: a real app looks once, on its first launch, and each lookup uses up the clicks it matched, so open the preview page again before each try.

## Guide

[https://deepbluelinks.io/docs/guides/maui](https://deepbluelinks.io/docs/guides/maui)
