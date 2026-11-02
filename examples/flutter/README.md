# Deep Blue Links Flutter example

This app opens its Deep Blue Links short links as universal links on iOS and App Links on Android, and through a custom URL scheme. It resolves each short link with `POST /api/links/resolve` and shows the link: its `id`, `longUrl`, `shortUrl` and `isSandbox`. On its first launch it looks for the link behind the install, reading the Play install referrer on Android and calling `POST /api/links/check-deferred` on iOS, and shows the result. The code is the [Flutter guide](https://deepbluelinks.io/docs/guides/flutter)'s: `lib/deep_blue_links.dart` is the guide's file, and `lib/main.dart` holds its link listener and the first-launch check.

## Requirements

Built with Flutter 3.47.7 (Dart 3.13.5), Xcode 27.0 and JDK 21. Flutter builds the iOS plugins with Swift Package Manager, so the project has no Podfile.

Universal links need a team in the Apple Developer Program, because personal teams can't use the Associated Domains capability.

## Configure

1. In the [Deep Blue Links dashboard](https://app.deepbluelinks.io), create the app and fill in its settings:
   - **iOS**: your team ID and the bundle ID, `com.example.deepbluelinks` unless you change it.
   - **Android**: the package name, `com.example.deepbluelinks` unless you change it, and the SHA-256 fingerprint of every key that signs a build you install. For debug builds, that's your debug keystore's:
     ```bash
     keytool -list -v -keystore ~/.android/debug.keystore -alias androiddebugkey -storepass android
     ```
   - **URL scheme**: `dblexample`, so the preview page's **Open the App** button opens the app.

   Then generate the app's sandbox key on its page.
2. Replace `demo` with your app's name in the dashboard, and the other placeholders with your values:

   | Value | Placeholder | Where |
   | --- | --- | --- |
   | API key | `YOUR_API_KEY` | `lib/config.dart`. Use the sandbox key, which starts with `sandbox_`, in debug builds and the live key in release builds. |
   | API URL | `https://thedbl.ink` | `lib/config.dart`. It's the API's address. |
   | Link domain | `demo.thedbl.ink` | `ios/Runner/Runner.entitlements` (`applinks:demo.thedbl.ink`) and `android/app/src/main/AndroidManifest.xml` (`android:host`). Add one entry per [custom domain](https://deepbluelinks.io/docs/custom-domains) in both files. |
   | URL scheme | `dblexample` | `lib/config.dart`, `ios/Runner/Info.plist` (`CFBundleURLSchemes`) and `android/app/src/main/AndroidManifest.xml` (`android:scheme`). |
   | iOS bundle ID | `com.example.deepbluelinks` | The Runner target's **Signing & Capabilities** in Xcode. |
   | Android application ID | `com.example.deepbluelinks` | `applicationId` in `android/app/build.gradle.kts`. |

3. Open `ios/Runner.xcworkspace` in Xcode, select the **Runner** target and choose your team under **Signing & Capabilities**. The Associated Domains capability is already there, from `Runner.entitlements`.

## Run

```bash
flutter pub get
flutter run
```

On iOS, a debug build only starts from `flutter run`. To open the app from a link while it isn't running, install a release build with `flutter run --release`.

## Test it

- Create a link with the sandbox key, as the [quickstart](https://deepbluelinks.io/docs/quickstart) shows. The app shows `isSandbox: true` for it.
- iOS universal links work on a device: paste a short link into Notes and tap it.
- On Android, check verification with `adb shell pm get-app-links com.example.deepbluelinks`, then open a link with `adb shell am start -a android.intent.action.VIEW -d "https://demo.thedbl.ink/Xy3kP9aQ"`.
- Open the URL scheme with `xcrun simctl openurl booted "dblexample://products/1234"` on the iOS Simulator, or `adb shell am start -a android.intent.action.VIEW -d "dblexample://products/1234"` on Android. The app shows the route `/products/1234` and doesn't call the API.
- The install referrer is only present for installs from Google Play, such as an internal testing track opened from a link's preview page.
- To test a deferred link on iOS, delete the app, open a short link in Safari on the device, then install and launch the app within 60 minutes on the same network. Turn off iCloud Private Relay for the test, because it hides the device's address from the preview page. [Deferred deep links](https://deepbluelinks.io/docs/deferred-deep-links) explains the match.
- The first-launch check runs once per install. **Check for a deferred link** runs it again. Each call uses up the clicks it matched, so open a short link in Safari again before you press it.

## Guide

[Flutter guide](https://deepbluelinks.io/docs/guides/flutter)
