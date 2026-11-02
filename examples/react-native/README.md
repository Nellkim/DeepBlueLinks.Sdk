# Deep Blue Links: React Native example

A React Native app that follows the [React Native guide](https://deepbluelinks.io/docs/guides/react-native). It opens from its Deep Blue Links short links (universal links on iOS, App Links on Android) and from its custom URL scheme, at launch and while it runs. It resolves each https link with `POST /api/links/resolve` and shows the link's `id`, `longUrl`, `shortUrl` and `isSandbox`, and the route an app would navigate to. On its first launch it looks for the link behind the install: the Play install referrer on Android, falling back to `POST /api/links/check-deferred` when the referrer isn't available yet, and `POST /api/links/check-deferred` on iOS. It shows that result and the last error. The code the guide describes is in `deepBlueLinks.ts` and `App.tsx`.

## Requirements

Built and checked with:

- Node 22.23.2 (React Native 0.88 needs Node 22.13 or later)
- React Native 0.88.1, with `react-native-device-info` 15.0.2 and `@react-native-async-storage/async-storage` 3.1.1
- Xcode 27.0 and CocoaPods 1.17
- JDK 21 and the Android SDK: compile SDK 37 and NDK 27.1.12297006, which Gradle installs when it's missing

Universal links need an iPhone or iPad and an Apple developer team. App Links work on a device or an emulator.

## Configure

1. In the [Deep Blue Links dashboard](https://app.deepbluelinks.io), add an app. Its name becomes the link host, `{name}.thedbl.ink`. Enter:
   - **iOS**: your team ID and the bundle ID, `com.example.deepbluelinks` or your own.
   - **Android**: the package name, `com.example.deepbluelinks` or your own, and the SHA-256 fingerprint of each key that signs the builds. Debug builds are signed with `android/app/debug.keystore`, whose SHA-256 is `FA:C6:17:45:DC:09:03:78:6F:B9:ED:E6:2A:96:2B:39:9F:73:48:F0:BB:6F:89:9B:83:32:66:75:91:03:3B:9C`. `cd android && ./gradlew signingReport` prints it. Installs from Google Play need the app signing key's fingerprint from the Play Console.
   - **URL scheme**: `dblexample`, or your own. The preview page's Open the App button uses it.

   Generate the sandbox key on the app's page.
2. Replace `demo` with the app's name in the link host `demo.thedbl.ink`:
   - `config.ts`: `LINK_DOMAIN`
   - `ios/DeepBlueLinksExample/DeepBlueLinksExample.entitlements`: `applinks:demo.thedbl.ink`
   - `android/app/src/main/AndroidManifest.xml`: `android:host` in the `autoVerify` intent filter

   Add one entry to each of the last two files per custom domain.
3. In `config.ts`, set `API_KEY` to the app's sandbox key, which starts with `sandbox_`, for debug builds; release builds use the live key. `API_URL` stays `https://thedbl.ink`.
4. If you use another URL scheme than `dblexample`, change it in:
   - `config.ts`: `URL_SCHEME`
   - `ios/DeepBlueLinksExample/Info.plist`: `CFBundleURLSchemes`
   - `android/app/src/main/AndroidManifest.xml`: `android:scheme` in the second intent filter
5. If you use another bundle ID or package than `com.example.deepbluelinks`, change it in:
   - `config.ts`: `APP_ID`
   - Xcode: the target's **Bundle Identifier** (`PRODUCT_BUNDLE_IDENTIFIER` in `ios/DeepBlueLinksExample.xcodeproj/project.pbxproj`)
   - `android/app/build.gradle`: `applicationId`
6. In Xcode, open `ios/DeepBlueLinksExample.xcworkspace` (after `pod install`, below), select the target, and choose your team under **Signing & Capabilities**. The Associated Domains entry is already there, from the entitlements file.

## Run

```bash
npm install
cd ios && pod install && cd ..
npm start
```

In another terminal:

```bash
npm run android
npm run ios
```

To run on an iPhone, select it in Xcode and run the `DeepBlueLinksExample` scheme.

## Test it

- iOS universal links work on a device: paste a short link into Notes and tap it.
- On Android, check verification with `adb shell pm get-app-links com.example.deepbluelinks`, then open a link with `adb shell am start -a android.intent.action.VIEW -d "https://demo.thedbl.ink/Xy3kP9aQ"`.
- The URL scheme opens the app with the path it carries, without a call to the API: `xcrun simctl openurl booted "dblexample://products/1234"` or `adb shell am start -a android.intent.action.VIEW -d "dblexample://products/1234"`.
- The install referrer is only present for installs from Google Play, such as an internal testing track opened from a link's preview page. Other installs, and a first launch that reads the referrer before Google Play has answered, fall back to `check-deferred` with the timezone alone.
- On iOS, delete the app, open a short link in Safari on the device so its preview page records the click, then install and launch the app from Xcode within 60 minutes on the same network.

The first launch runs the lookup once and stores a flag with AsyncStorage; deleting the app clears it. **Check for a deferred link** runs the same lookup again, for testing only: on iOS a second call answers "No deferred link", because the first used up the clicks.

## Guide

[React Native guide](https://deepbluelinks.io/docs/guides/react-native)
