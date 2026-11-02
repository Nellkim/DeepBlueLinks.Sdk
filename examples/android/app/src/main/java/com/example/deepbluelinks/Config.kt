package com.example.deepbluelinks

// The values to change for your app. AndroidManifest.xml repeats LINK_DOMAIN and URL_SCHEME,
// and app/build.gradle.kts holds the application id: change them there too.
object Config {
    const val API_URL = "https://thedbl.ink"

    // The sandbox key (it starts with sandbox_) in debug builds, the live key in release builds.
    const val API_KEY = "YOUR_API_KEY"

    const val LINK_DOMAIN = "demo.thedbl.ink"

    const val URL_SCHEME = "dblexample"
}
