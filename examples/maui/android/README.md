# Deep Blue Links .NET for Android example

The [.NET MAUI guide](https://deepbluelinks.io/docs/guides/maui)'s code in a native .NET for Android app, without MAUI. The app opens from its short links as verified App Links and from its custom URL scheme, both at cold start (`OnCreate`) and while it runs (`OnNewIntent`). It resolves each short link with `POST /api/links/resolve` and shows the link's `id`, `longUrl`, `shortUrl` and `isSandbox`, along with the route the app would open. On its first launch, it reads the Play install referrer, resolves the referrer's `link` value, and shows the result. A **Check for a deferred link** button runs the same lookup again. It's a testing aid: a real app looks only once. The screen also shows the last error.

`DeepBlueLinks.cs`, `InstallReferrer.cs` and `DeepBlueLinksConfig.cs` are the .NET MAUI example's files, keeping only their Android code. In place of MAUI's lifecycle events and `OnAppLinkRequestReceived`, `MainActivity` handles the link intent itself. It also keeps the first-launch flag in `SharedPreferences`, where the MAUI example uses `Preferences`.

## Requirements

Built with:

- .NET SDK 10.0.403
- The `android` workload from workload set 10.0.401.1 (.NET for Android 36.1.69)
- The Android SDK with the API 36 platform, and JDK 21. The app runs on Android 7.0 (API 24) and newer.

To test the install referrer, you need a device or emulator with Google Play.

## Configure

1. Create the app in the [Deep Blue Links dashboard](https://app.deepbluelinks.io), and fill in its Android section:
   - **Package name**: `com.example.deepbluelinks`, or your own application ID (step 2).
   - **SHA-256 certificate fingerprints**: the fingerprint of every key that signs the app. Debug builds are signed with `~/Library/Application Support/Xamarin/Mono for Android/debug.keystore` on macOS. Print its fingerprint with `keytool -list -v -keystore "<path>" -alias androiddebugkey -storepass android`. For Play builds, add the Play app signing key's fingerprint too.
   - **URL scheme**: `dblexample`, or your own.
   - **API keys**: generate the sandbox key.
2. Replace the placeholders. Replace `demo` with your app's name in the Deep Blue Links dashboard.

   | Value | Placeholder | Where |
   |---|---|---|
   | API URL | `https://thedbl.ink` | `DeepBlueLinksConfig.cs` (`ApiUrl`) |
   | API key | `YOUR_API_KEY` | `DeepBlueLinksConfig.cs` (`ApiKey`) |
   | Link domain | `demo.thedbl.ink` | `DeepBlueLinksConfig.cs` (`LinkDomain`, which the verified intent filter in `MainActivity.cs` uses) |
   | URL scheme | `dblexample` | `DeepBlueLinksConfig.cs` (`UrlScheme`, which the second intent filter uses) |
   | Application ID | `com.example.deepbluelinks` | `DeepBlueLinksExample.csproj` (`ApplicationId`), the Android package name |

   Use the app's sandbox key, which starts with `sandbox_`, in debug builds, and the live key in release builds. See [sandbox links](https://deepbluelinks.io/docs/sandbox).
3. For each [custom domain](https://deepbluelinks.io/docs/custom-domains), add an intent filter to `MainActivity`.

The first-launch flag lives in `deep_blue_links.xml`, which `Resources/xml/backup_rules.xml` and `Resources/xml/data_extraction_rules.xml` leave out of Auto Backup. Otherwise, a reinstall would restore the flag and never read the new install's referrer. If you rename `PreferencesName` in `MainActivity.cs`, change both files to match.

## Run

```bash
dotnet build -t:Run -f net10.0-android
```

This builds the app, installs it on the connected device or running emulator, and launches it.

## Test it

- Check verification with `adb shell pm get-app-links com.example.deepbluelinks`. The host should show `verified`, and after you change your app's settings, `adb shell pm verify-app-links --re-verify com.example.deepbluelinks` checks again. Then open a link with `adb shell am start -a android.intent.action.VIEW -d "https://demo.thedbl.ink/Xy3kP9aQ"`, using one of your links' short codes. Run the command again while the app is open, and the link arrives through `OnNewIntent`.
- Open the URL scheme with `adb shell am start -a android.intent.action.VIEW -d "dblexample://products/1234"`. The app shows the route, `/products/1234`, without calling the API, as the guide routes scheme URLs.
- The install referrer is only present for installs from Google Play, such as an internal testing track opened from a link's preview page. A build you install yourself shows "No deferred link". `adb shell pm clear com.example.deepbluelinks` resets the first-launch flag.

## Guide

[https://deepbluelinks.io/docs/guides/maui](https://deepbluelinks.io/docs/guides/maui)
