# .NET examples

Three apps with the [.NET guide](https://deepbluelinks.io/docs/guides/maui)'s code, one per kind of .NET mobile app. The guide's last section covers the two apps without MAUI. They share the same API client (`DeepBlueLinks.cs`) and configuration (`DeepBlueLinksConfig.cs`), and the Android ones the same install-referrer reader (`InstallReferrer.cs`). Only the way each receives its links and shows them differs.

| Folder | App | Links arrive in |
|---|---|---|
| [`multiplatform/`](multiplatform) | .NET MAUI, one project for iOS and Android | `Application.OnAppLinkRequestReceived`, forwarded from each platform's lifecycle events |
| [`android/`](android) | .NET for Android, without MAUI | `MainActivity.OnCreate` and `OnNewIntent` |
| [`ios/`](ios) | .NET for iOS, without MAUI | The scene delegate's user activities and URL contexts |

Each folder's README lists the values to change, how to run it and how to test it.
