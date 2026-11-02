namespace DeepBlueLinksExample;

// Info.plist declares a scene manifest, so iOS delivers links to SceneDelegate,
// never to ContinueUserActivity or OpenUrl here.
[Register("AppDelegate")]
public class AppDelegate : UIApplicationDelegate
{
    public override bool FinishedLaunching(UIApplication application, NSDictionary? launchOptions) => true;

    public override UISceneConfiguration GetConfiguration(UIApplication application, UISceneSession connectingSceneSession, UISceneConnectionOptions options) =>
        new("Default Configuration", connectingSceneSession.Role);
}
