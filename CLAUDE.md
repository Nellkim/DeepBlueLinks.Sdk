# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this is

Runnable example apps for Deep Blue Links, one per platform its documentation has a guide for. Each guide at `https://deepbluelinks.io/docs/guides/<guide>` opens with a tip linking to its example here, so the two are read side by side.

- The guides are MDX in the `DeepBlueLinks.web` repository, `marketing/content/docs/guides/`. The API reference text comes from the API's `ApiDocs.cs` in the `DeepBlueLinks.Api` repository.
- The examples call the public API only: `POST /api/links/resolve` and `POST /api/links/check-deferred` from the apps, and `POST`, `PUT` and `DELETE /api/links` from the server examples, all with the `Api-Key` header.

| Folder | Guide | App |
|---|---|---|
| `examples/ios` | `ios` | SwiftUI, hand-written `.xcodeproj` (file-system-synchronized group) |
| `examples/android` | `android` | Kotlin, one activity with views, Gradle Kotlin DSL |
| `examples/flutter` | `flutter` | Flutter, iOS and Android |
| `examples/react-native` | `react-native` | React Native community CLI template, TypeScript |
| `examples/capacitor` | `capacitor` | Capacitor with a Vite + TypeScript page, iOS through Swift Package Manager |
| `examples/maui/multiplatform` | `maui` | .NET MAUI, iOS and Android |
| `examples/maui/android` | `maui` | .NET for Android, no MAUI |
| `examples/maui/ios` | `maui` | .NET for iOS, no MAUI |
| `examples/server` | `server` | curl, Node.js, Python and C# scripts |

## Rules

- **An example follows its guide's code**: the same file names, type and function names, API calls and packages. A change to a guide's code goes into its example in the same change, and a fix found in an example goes into its guide. Readers map one onto the other line by line, so a difference reads as a mistake.
  - An example adds only what makes it runnable and visible: one screen with the link that opened the app, the route, the deferred-link result and the last error, and a **Check for a deferred link** button labelled as a testing aid.
  - The three .NET examples share `DeepBlueLinks.cs`, `DeepBlueLinksConfig.cs` and, on Android, `InstallReferrer.cs`; a change to one goes into the others.
- **Every app example behaves the same way:**
  - **Opening links:** it resolves a universal link or App Link with `resolve`, at cold start and while it runs. A URL scheme link (`dblexample://products/1234`) is routed as `/` + host + path without resolving, because it carries the destination, not a short code.
  - **Failures:** a 404 or a failed call routes by the URL itself and shows the error. Nothing crashes on a network error or a 401.
  - **First launch:** it looks for the link behind the install once and stores a flag. On Android it reads the Play install referrer, and on iOS and elsewhere it calls `check-deferred` with `device`. A referrer that can't be read counts as no link.
  - **Android flag and backups:** the flag is excluded from Android's Auto Backup, or a reinstall restores it and the new install's referrer is never read.
- **Placeholders are the docs' own**, so a reader replaces the same values in both:
  - the link domain `demo.thedbl.ink` (replace `demo` with the app's name);
  - the API URL `https://thedbl.ink`;
  - the key `YOUR_API_KEY` (the sandbox key, `sandbox_…`, in debug builds);
  - the URL scheme `dblexample`;
  - the bundle and application id `com.example.deepbluelinks`.

  Every value lives in one config file per example. The platform files that can't read it (entitlements, `Info.plist`, `AndroidManifest.xml`, the csproj or Gradle file) repeat it, and each README lists every place.
- **Never commit:**
  - **An API key or an Apple development team id.** `flutter create` and Xcode fill in this Mac's team; remove it.
  - **Build output:** `bin/`, `obj/`, `build/`, `.gradle/`, `.kotlin/`, `DerivedData/`, `node_modules/`, `Pods/`, `dist/`.
  - **Machine files:** `local.properties`, `.xcode.env.local`, and the `Package.resolved` that `xcodebuild` writes into a project's workspace.
- **Each example ignores its own files** in its folder's `.gitignore`. The root `.gitignore` lists only OS and editor files, because a broad rule hides files an example needs: Visual Studio's `[Dd]ebug/` matches Android's `app/src/debug/AndroidManifest.xml`.
- **Each example has a README** in this order:
  1. What it shows.
  2. Requirements, with the versions it was built with.
  3. Configure: every value and file, and registering the app in the dashboard.
  4. Run.
  5. Test it.
  6. The guide's link.
  Plain, factual sentences.
- **Name only Deep Blue Links,** the platform vendors (Apple, Google, Microsoft) and the open-source packages an example uses.
- **Commits**: a short subject, plus a few lines only when the reason isn't visible in the diff. No AI attribution.

## Building

Build every example you change, and delete its build output afterwards. There are no tests: a build with no warnings from our code, plus a run on a simulator or emulator where the change touches link handling, is the check.

| Example | Commands |
|---|---|
| `ios` | `xcodebuild -project DeepBlueLinksExample.xcodeproj -scheme DeepBlueLinksExample -sdk iphonesimulator -destination 'generic/platform=iOS Simulator' CODE_SIGNING_ALLOWED=NO build` |
| `android` | `./gradlew assembleDebug` with JDK 21 and `ANDROID_HOME` set |
| `flutter` | `flutter analyze`, `flutter build apk --debug`, `flutter build ios --simulator --debug --no-codesign`, then `flutter clean` |
| `react-native` | Node 22.13 or later. `npm ci`, `npx tsc --noEmit`, `npm run lint`, `cd android && ./gradlew assembleDebug`, `cd ios && pod install`, then `xcodebuild -workspace DeepBlueLinksExample.xcworkspace -scheme DeepBlueLinksExample -sdk iphonesimulator -destination 'generic/platform=iOS Simulator' CODE_SIGNING_ALLOWED=NO build` |
| `capacitor` | Node 22.18 or later. `npm ci`, `npm run build`, `npx cap sync`, `cd android && ./gradlew assembleDebug`, then `xcodebuild -project ios/App/App.xcodeproj -scheme App -sdk iphonesimulator -destination 'generic/platform=iOS Simulator' CODE_SIGNING_ALLOWED=NO build` |
| `maui/multiplatform` | `dotnet build -f net10.0-android`, then `dotnet build -f net10.0-ios -p:RuntimeIdentifier=iossimulator-arm64 -p:CodesignRequireProvisioningProfile=false` |
| `maui/android` | `dotnet build -f net10.0-android`, and with `-c Release` |
| `maui/ios` | `dotnet build -f net10.0-ios -p:RuntimeIdentifier=iossimulator-arm64 -p:CodesignRequireProvisioningProfile=false` |
| `server` | `DBL_API_KEY=… DBL_APP_ID=… ./curl/links.sh`, `node node/links.mjs`, `python python/links.py`, `dotnet run --project dotnet` |

Toolchain facts the builds depend on:

- **Xcode 27 builds only for iOS 15 and later.** CocoaPods resource bundles with lower targets stop the React Native build, so its Podfile's `post_install` raises every pod target to React Native's minimum.
- **Flutter and Capacitor use Swift Package Manager on iOS**: neither has a Podfile, and Capacitor has no `.xcworkspace`.
- **.NET for Android keeps the default `TrimMode` (`partial`).** With `full`, System.Text.Json's reflection is trimmed away and the API client fails at runtime in Release builds.
- **An `.entitlements` file needs a provisioning profile** in a signed .NET iOS build, so simulator builds pass `-p:CodesignRequireProvisioningProfile=false`.

## Testing against the API

- **Check the routes before debugging an example.** The guides' code reads a 404 as "no link", so an API without the `api/links/*` routes looks like a link that never matches. With a dummy key, `POST /api/links/resolve` must answer 401, not 404; `https://dev.thedbl.ink` is the dev API.
- **Universal links and App Links need a device** signed with a team and key the app's dashboard settings list. The simulators and emulators can only check the URL scheme, the first-launch flow and error handling.
- **The install referrer needs an install from Google Play**, such as an internal testing track opened from a link's preview page.
- **A deferred match on iOS** needs the link opened in Safari on the same device and connection, and the app launched within an hour.
