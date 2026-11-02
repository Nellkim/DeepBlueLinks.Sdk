# Deep Blue Links examples

Runnable example apps for [Deep Blue Links](https://deepbluelinks.io), one per platform. Each follows its guide in the [documentation](https://deepbluelinks.io/docs) line by line, so you can read the guide and run the same code.

| Example | Platform | Guide |
|---|---|---|
| [`examples/ios`](examples/ios) | iOS, Swift and SwiftUI | [iOS](https://deepbluelinks.io/docs/guides/ios) |
| [`examples/android`](examples/android) | Android, Kotlin | [Android](https://deepbluelinks.io/docs/guides/android) |
| [`examples/flutter`](examples/flutter) | Flutter, iOS and Android | [Flutter](https://deepbluelinks.io/docs/guides/flutter) |
| [`examples/react-native`](examples/react-native) | React Native, iOS and Android | [React Native](https://deepbluelinks.io/docs/guides/react-native) |
| [`examples/capacitor`](examples/capacitor) | Capacitor, iOS and Android | [Capacitor](https://deepbluelinks.io/docs/guides/capacitor) |
| [`examples/maui/multiplatform`](examples/maui/multiplatform) | .NET MAUI, iOS and Android | [.NET](https://deepbluelinks.io/docs/guides/maui) |
| [`examples/maui/android`](examples/maui/android) | .NET for Android, without MAUI | [.NET](https://deepbluelinks.io/docs/guides/maui) |
| [`examples/maui/ios`](examples/maui/ios) | .NET for iOS, without MAUI | [.NET](https://deepbluelinks.io/docs/guides/maui) |
| [`examples/server`](examples/server) | curl, Node.js, Python and C# | [Server](https://deepbluelinks.io/docs/guides/server) |

## What the app examples do

Every app example is one screen that shows:

- **The link that opened the app.** A universal link on iOS, an App Link on Android or the app's own URL scheme opens the app, which sends the URL to [`POST /api/links/resolve`](https://deepbluelinks.io/docs/api/resolve-link) and shows the link's `id`, `longUrl`, `shortUrl` and `isSandbox`.
- **The link behind the install.** On its first launch the app asks which link brought the user to the store: on Android it reads the link from the Play install referrer, and elsewhere it calls [`POST /api/links/check-deferred`](https://deepbluelinks.io/docs/api/check-deferred-link). See [deferred deep links](https://deepbluelinks.io/docs/deferred-deep-links).
- **A button to look again**, for testing, because a real app looks only once.

The server examples create, update and delete a link from a backend.

## Before you run one

1. Create your app in the [dashboard](https://app.deepbluelinks.io) with your iOS team and bundle ids and your Android package and SHA-256 certificate fingerprints, and generate its API keys. The [quickstart](https://deepbluelinks.io/docs/quickstart) walks through it.
2. In the example, replace `demo` with your app's name, `YOUR_API_KEY` with its sandbox key, which starts with `sandbox_`, and `com.example.deepbluelinks` with your own bundle id and package. Each example's README lists every file to change.

Use the sandbox key in debug builds and the live key only in release builds: links created with the sandbox key are marked as sandbox links, and the key can never change a live one. See [sandbox links](https://deepbluelinks.io/docs/sandbox).

## The DeepBlueLinks.Maui package

The `DeepBlueLinks.Maui` NuGet package is no longer developed. Its published versions keep working, and [`examples/maui`](examples/maui) shows the same integration with the current API and no package, in a .NET MAUI app and in native .NET for Android and .NET for iOS apps.
