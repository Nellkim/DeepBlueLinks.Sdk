# Deep Blue Links iOS example

A SwiftUI app that does what the [iOS guide](https://deepbluelinks.io/docs/guides/ios) teaches, with the guide's code. A universal link that opens the app, at launch or while it runs, is resolved with `POST /api/links/resolve`. A link through the app's custom URL scheme is routed by its own path, without being resolved. On the first launch the app asks `POST /api/links/check-deferred` for the link behind the install. The screen shows the link that opened the app, the result of the deferred check, where the app routes, and the last error.

## Requirements

- Xcode 27.0 or later. The example was built with Xcode 27.0 (27A266a).
- iOS 17 or later.
- An Apple developer team and a device, because universal links don't open apps in the Simulator.

## Configure

1. In the [Deep Blue Links dashboard](https://app.deepbluelinks.io), add an app with your Apple **team ID** and the **bundle ID** you'll use for this example. Set its URL scheme to `dblexample`, or to your own scheme (steps 3 and 5). Generate the app's sandbox API key, which starts with `sandbox_`.
2. Open `DeepBlueLinksExample.xcodeproj`. Select the `DeepBlueLinksExample` target, open **Signing & Capabilities**, choose your **Team** and replace the bundle identifier `com.example.deepbluelinks` with the bundle ID from step 1.
3. In `DeepBlueLinksExample/Config.swift`:
   - `apiKey`: replace `YOUR_API_KEY` with the sandbox key for debug builds, and with the app's live key for release builds.
   - `linkDomain`: replace `demo` in `demo.thedbl.ink` with your app's name in the Deep Blue Links dashboard.
   - `urlScheme`: `dblexample`, unless you picked another scheme.
   - `apiURL`: keep `https://thedbl.ink`.
4. In `DeepBlueLinksExample/DeepBlueLinksExample.entitlements`, replace `demo` in `applinks:demo.thedbl.ink` with your app's name. Add one `applinks:` entry per custom domain, such as `applinks:link.example.com`.
5. In `DeepBlueLinksExample/Info.plist`, the URL type's scheme `dblexample` must match `urlScheme` and the scheme in the dashboard.

## Run

Choose the `DeepBlueLinksExample` scheme and your device, then **Product › Run**.

To check that it builds without a team:

```bash
xcodebuild -project DeepBlueLinksExample.xcodeproj -scheme DeepBlueLinksExample \
  -sdk iphonesimulator -destination 'generic/platform=iOS Simulator' CODE_SIGNING_ALLOWED=NO build
```

## Test it

- Universal links work on a device, not the Simulator. Paste a short link into Notes and tap it; long-press it to choose between the app and Safari. The screen shows the resolved link's `id`, `longUrl`, `shortUrl` and `isSandbox`, and the route taken from `longUrl`.
- Typing the link into Safari's address bar opens the page, not the app, by design.
- In **Settings › Developer › Universal Links**, **Diagnostics** checks a URL against your installed app.
- iOS fetches `apple-app-site-association` through Apple's CDN, which can lag behind a change to your app's settings. While testing, `applinks:demo.thedbl.ink?mode=developer` with **Associated Domains Development** turned on fetches it directly.
- The URL scheme works in the Simulator too: `xcrun simctl openurl booted "dblexample://products/1234"` shows the route `/products/1234`, and nothing is resolved.
- To try a deferred link, delete the app, open a short link in Safari on the device so the preview page records the click, then run the app from Xcode on the same network within 60 minutes. The first launch shows the link. A 404 shows as "No deferred link", which is normal: most launches don't follow a link. [Deferred deep links](https://deepbluelinks.io/docs/deferred-deep-links#when-a-click-cant-be-matched) explains when a match isn't possible.
- **Check for a deferred link** runs the same check again. It's a testing aid: a real app checks once, on its first launch, and each check uses up the clicks it matched.

## Guide

[iOS guide](https://deepbluelinks.io/docs/guides/ios)
