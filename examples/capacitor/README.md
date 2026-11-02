# Deep Blue Links: Capacitor example

A Capacitor app with one screen that follows the [Capacitor guide](https://deepbluelinks.io/docs/guides/capacitor). When a universal link, App Link or the app's URL scheme opens it, from a cold start or while it runs, it resolves the link with `POST /api/links/resolve`. On its first launch it looks for the link behind the install: through the Play install referrer on Android, and with `POST /api/links/check-deferred` elsewhere. The screen shows the link that opened the app, the deferred link, the route the app would open and the last error. `src/deepBlueLinks.ts` holds the guide's code. `src/router.ts` stands in for your app's navigation, and `src/view.ts` draws the screen.

## Requirements

Built with:

- Node.js 22.23.2 and npm 10.9.8. The Capacitor CLI needs Node.js 22.18 or later here: TypeScript 7 has no compiler API, so the CLI loads `capacitor.config.ts` with Node's own TypeScript support.
- Capacitor 8.6.0, `@capacitor/app` 8.1.2 and `@capgo/capacitor-install-referrer` 8.1.15, with Vite 8.3.5 and TypeScript 7.0.2.
- Xcode 27.0. The iOS project uses Swift Package Manager, not CocoaPods, and targets iOS 15.
- JDK 21 and the Android SDK with platform 36. The project uses Gradle 8.14.3 and targets Android 7.0 (API 24) and later.

## Configure

1. Register the app in the [Deep Blue Links dashboard](https://app.deepbluelinks.io). Its name sets the link host, `{name}.thedbl.ink`. Enter:
   - the iOS team ID and bundle ID;
   - the Android package name and the SHA-256 fingerprint of every key that signs a build you install. For the debug keystore, the fingerprint is the `SHA256` line of:

     ```bash
     keytool -list -v -keystore ~/.android/debug.keystore -alias androiddebugkey -storepass android -keypass android
     ```

   - the URL scheme, `dblexample` here, which the preview page's **Open the App** button uses.

   Then generate the app's sandbox key, which starts with `sandbox_`, for debug builds. Release builds use the live key.

2. Set the values in `src/config.ts`. `capacitor.config.ts` reads `APP_ID` and `APP_NAME` from it.

   | Constant | Placeholder | Value |
   | --- | --- | --- |
   | `API_URL` | `https://thedbl.ink` | The Deep Blue Links API |
   | `API_KEY` | `YOUR_API_KEY` | The sandbox key in debug builds, the live key in release builds |
   | `LINK_DOMAIN` | `demo.thedbl.ink` | Your link host: replace `demo` with the app's name in the dashboard |
   | `URL_SCHEME` | `dblexample` | The URL scheme in the app's settings |
   | `APP_ID` | `com.example.deepbluelinks` | The iOS bundle ID and the Android application ID |
   | `APP_NAME` | `DBL Example` | The name under the app's icon |

3. Change the same values in the native projects, which `npx cap sync` doesn't update:

   | Value | File | Where |
   | --- | --- | --- |
   | Link host | `ios/App/App/App.entitlements` | `applinks:demo.thedbl.ink`, plus one `applinks:` entry per custom domain |
   | Link host | `android/app/src/main/AndroidManifest.xml` | `android:host` in the `autoVerify` filter, plus one `<data>` per custom domain |
   | URL scheme | `ios/App/App/Info.plist` | `CFBundleURLSchemes` |
   | URL scheme | `android/app/src/main/AndroidManifest.xml` | `android:scheme` in the second filter |
   | App ID | `ios/App/App.xcodeproj/project.pbxproj` | `PRODUCT_BUNDLE_IDENTIFIER`, in Xcode the App target's **General** > **Bundle Identifier** |
   | App ID | `android/app/build.gradle` | `applicationId` |
   | App ID | `android/app/src/main/res/values/strings.xml` | `package_name` |
   | App name | `ios/App/App/Info.plist` | `CFBundleDisplayName` |
   | App name | `android/app/src/main/res/values/strings.xml` | `app_name` and `title_activity_main` |

4. In Xcode, select your team under the App target's **Signing & Capabilities**. The **Associated Domains** capability is already there, from `App.entitlements`.

## Run

```bash
npm install
npm run build && npx cap sync
```

Then open the native project and run it from the IDE:

```bash
npx cap open ios
npx cap open android
```

Or build and run on a device or emulator in one step with `npx cap run ios` or `npx cap run android`. The iOS project is `ios/App/App.xcodeproj`; there is no `.xcworkspace`.

## Test it

- iOS universal links work on a device: paste a short link into Notes and tap it.
- On Android, check verification with `adb shell pm get-app-links com.example.deepbluelinks`, then open a link with `adb shell am start -a android.intent.action.VIEW -d "https://demo.thedbl.ink/Xy3kP9aQ"`, using one of your app's short codes.
- The install referrer is only present for installs from Google Play, such as an internal testing track opened from a link's preview page. A build installed from Android Studio has none, so its check shows **No deferred link** or the plugin's error.
- The URL scheme: run `xcrun simctl openurl booted "dblexample://products/1234"` on the iOS Simulator, or `adb shell am start -a android.intent.action.VIEW -d "dblexample://products/1234"` on Android. The route shows `/products/1234`, and the app doesn't call the API.
- The deferred link on iOS: delete the app, open a short link in Safari on the device so its preview page records the click, then install and launch the app from Xcode on the same network within 60 minutes, with iCloud Private Relay off. The first launch shows the link.
- **Check for a deferred link** runs the same lookup again. A real app runs it once. A second `check-deferred` finds nothing, because a match uses up the clicks it saw. To repeat the first launch, delete the app or clear its data.

## Guide

[Capacitor guide](https://deepbluelinks.io/docs/guides/capacitor)
