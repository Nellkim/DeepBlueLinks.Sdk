# Deep Blue Links Android example

A single-screen Kotlin app that follows the [Android guide](https://deepbluelinks.io/docs/guides/android). It opens your app's short links as verified App Links and through its custom URL scheme, both at cold start and while it runs (`onNewIntent`). It resolves each short link with `POST /api/links/resolve` and shows its `id`, `longUrl`, `shortUrl` and `isSandbox`, along with the route the app would open. On its first launch it reads the Play Install Referrer, resolves the referrer's `link` value, and shows that result too. A "Check for a deferred link" button runs the same lookup again. It's a testing aid: a real app runs it only once. The screen also shows the last error.

## Requirements

- Android Studio 2026.2, which uses Android Gradle Plugin 9.5.0 and its built-in Kotlin 2.2.10.
- JDK 21. The example was built with Temurin 21, and AGP 9 needs JDK 17 or newer.
- Android SDK Platform 37.2 (`compileSdk` 37.2, `targetSdk` 37). The app runs on Android 7.0 (API 24) and newer.
- The Gradle wrapper downloads Gradle 9.7.1.
- To test the install referrer, a device or emulator with Google Play.

## Configure

1. In the [Deep Blue Links dashboard](https://app.deepbluelinks.io), open your app's settings and fill in its Android section:
   - **Package name**: the application ID, `com.example.deepbluelinks` unless you change it in step 3.
   - **SHA-256 certificate fingerprints**: the fingerprint of every key that signs a build you'll install. For debug builds, that's your debug keystore. Print its fingerprint with `./gradlew signingReport` (the `SHA-256` line under `Variant: debug`) or with:

     ```bash
     keytool -list -v -keystore ~/.android/debug.keystore -alias androiddebugkey -storepass android -keypass android
     ```

     With Play App Signing, also add the app signing key's fingerprint from the Play Console.
   - **URL scheme** (optional): `dblexample`, or the scheme you set in step 2.

   Generate a sandbox key on the same page.
2. Replace `demo` with your app's name in the Deep Blue Links dashboard, and `YOUR_API_KEY` with its key. Use the sandbox key, which starts with `sandbox_`, in debug builds and the live key in release builds.
   - `app/src/main/java/com/example/deepbluelinks/Config.kt`:
     - `API_URL`: the Deep Blue Links API, `https://thedbl.ink`.
     - `API_KEY`: `YOUR_API_KEY`.
     - `LINK_DOMAIN`: `demo.thedbl.ink`.
     - `URL_SCHEME`: `dblexample`.
   - `app/src/main/AndroidManifest.xml`: `android:host="demo.thedbl.ink"` and `android:scheme="dblexample"`. Add a `<data>` element for each custom domain.
3. To use your own application ID, change `applicationId = "com.example.deepbluelinks"` in `app/build.gradle.kts`, and use the same ID as the package name in the dashboard and in the `adb` commands below.

## Run

Open the folder in Android Studio and run the `app` configuration, or install the debug build on a connected device or emulator:

```bash
export ANDROID_HOME=~/Library/Android/sdk   # or set sdk.dir in local.properties
./gradlew installDebug
```

## Test it

- Install a build signed with a key whose fingerprint is in your app's settings, then check the verification:

  ```bash
  adb shell pm get-app-links com.example.deepbluelinks
  ```

  The host should show `verified`. After changing your settings, `adb shell pm verify-app-links --re-verify com.example.deepbluelinks` checks again.
- Open a short link from another app, such as a chat, or with `adb shell am start -a android.intent.action.VIEW -d "https://demo.thedbl.ink/Xy3kP9aQ"`. The screen shows the resolved link and its route. Open a second link while the app is running to see it arrive through `onNewIntent`.
- Open a URL scheme link with `adb shell am start -a android.intent.action.VIEW -d "dblexample://products/1234"`. The app routes it to `/products/1234` without calling the API.
- To test the referrer, the app must be installed from Google Play, for example from an internal testing track, through a link's preview page. The first launch shows the link under "Deferred link (first launch)". A build you install yourself shows "No deferred link". `adb shell pm clear com.example.deepbluelinks` resets the first-launch flag.

## Guide

https://deepbluelinks.io/docs/guides/android
