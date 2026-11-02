// The values to change for your app.
//
// The platform files repeat some of them, because Xcode and Gradle read those
// files, not this one (README.md lists every place):
// - link domain demo.thedbl.ink: ios/Runner/Runner.entitlements and
//   android/app/src/main/AndroidManifest.xml
// - URL scheme dblexample: ios/Runner/Info.plist and AndroidManifest.xml
// - iOS bundle ID com.example.deepbluelinks: the Runner target in Xcode
// - Android application ID com.example.deepbluelinks:
//   android/app/build.gradle.kts

const apiUrl = 'https://thedbl.ink';

// The app's sandbox key (sandbox_...) in debug builds, its live key in release builds.
const apiKey = 'YOUR_API_KEY';

const urlScheme = 'dblexample';
