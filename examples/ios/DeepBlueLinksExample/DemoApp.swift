import SwiftUI

@main
struct DemoApp: App {
    @State private var model = AppModel()

    var body: some Scene {
        WindowGroup {
            ContentView(model: model)
                .onOpenURL { url in
                    Task {
                        await model.open(url)
                    }
                }
                .task {
                    await model.checkDeferredLinkOnFirstLaunch()
                }
        }
    }
}
