# Deep Blue Links .NET MAUI example

A .NET MAUI app for iOS and Android that does what the [.NET MAUI guide](https://deepbluelinks.io/docs/guides/maui) teaches, with the same code. The app opens from its short links (universal links on iOS, App Links on Android) and from its custom URL scheme. Each link goes to `Application.OnAppLinkRequestReceived`, the app resolves it with `POST /api/links/resolve`, and shows the result. On its first launch, the app looks for the link behind the install. On Android it reads the Play install referrer. On iOS it calls `POST /api/links/check-deferred` with the timezone, screen and language. Its one page shows the link that opened the app, the deferred-link result and the last error.

## Requirements

Built with:

- .NET SDK 10.0.403
- The `maui`, `ios` and `android` workloads from workload set 10.0.401.1 (.NET MAUI 10.0.110, .NET for iOS 27.0.10722, .NET for Android 36.1.69)
- Xcode 27.0, for iOS

Universal links need an iPhone or iPad and an Apple developer account.

## Configure

1. Create the app in the [Deep Blue Links dashboard](https://app.deepbluelinks.io):
   - **iOS**: your Apple team ID and the bundle ID.
   - **Android**: the package name and the SHA-256 fingerprint of every key that signs the app. Debug builds are signed with `~/Library/Application Support/Xamarin/Mono for Android/debug.keystore` on macOS. Print its fingerprint with `keytool -list -v -keystore "<path>" -alias androiddebugkey -storepass android`. For Play builds, add the Play app signing key's fingerprint too.
   - **URL scheme**: `dblexample`, or your own.
   - **API keys**: generate the sandbox key.
2. Replace the placeholders. Replace `demo` with your app's name in the Deep Blue Links dashboard.

   | Value | Placeholder | Where |
   |---|---|---|
   | API URL | `https://thedbl.ink` | `DeepBlueLinksConfig.cs` (`ApiUrl`) |
   | API key | `YOUR_API_KEY` | `DeepBlueLinksConfig.cs` (`ApiKey`) |
   | Link domain | `demo.thedbl.ink` | `DeepBlueLinksConfig.cs` (`LinkDomain`, which the Android intent filter in `Platforms/Android/MainActivity.cs` uses) and `Platforms/iOS/Entitlements.plist` |
   | URL scheme | `dblexample` | `DeepBlueLinksConfig.cs` (`UrlScheme`, which the second Android intent filter uses) and `Platforms/iOS/Info.plist` (`CFBundleURLSchemes`) |
   | Application ID | `com.example.deepbluelinks` | `DeepBlueLinksExample.csproj` (`ApplicationId`), which is both the iOS bundle ID and the Android package |

   Use the app's sandbox key, which starts with `sandbox_`, in debug builds, and the live key in release builds. See [sandbox links](https://deepbluelinks.io/docs/sandbox).
3. For each [custom domain](https://deepbluelinks.io/docs/custom-domains), add an `applinks:` entry to `Platforms/iOS/Entitlements.plist` and an intent filter to `MainActivity`.
4. On iOS, enable the **Associated Domains** capability on the App ID in your Apple developer account, and download the updated provisioning profile.

## Run

```bash
dotnet build -t:Run -f net10.0-android
dotnet build -t:Run -f net10.0-ios -p:RuntimeIdentifier=ios-arm64 -p:_DeviceName=<device UDID>
```

For the iOS simulator, use `-p:RuntimeIdentifier=iossimulator-arm64`. If you have no provisioning profile for the bundle ID, add `-p:CodesignRequireProvisioningProfile=false`.

## Test it

- iOS universal links work on a device: paste a short link into Notes and tap it.
- On Android, check verification with `adb shell pm get-app-links com.example.deepbluelinks`. Then open a link with `adb shell am start -a android.intent.action.VIEW -d "https://demo.thedbl.ink/Xy3kP9aQ"`, using one of your links' short codes.
- The install referrer is only present for installs from Google Play, such as an internal testing track opened from a link's preview page.
- Open the URL scheme with `adb shell am start -a android.intent.action.VIEW -d "dblexample://products/1234"` or `xcrun simctl openurl booted "dblexample://products/1234"`. The app shows the route, `/products/1234`, without calling the API, as the guide routes scheme URLs.
- To test a deferred link on iOS, delete the app and open a short link's preview page in Safari on the device. Then install and launch the app within 60 minutes, on the same network. **Check for a deferred link** runs the lookup again. Each lookup uses up the clicks it matched, so open the preview page again before each try.

## Guide

[https://deepbluelinks.io/docs/guides/maui](https://deepbluelinks.io/docs/guides/maui)
