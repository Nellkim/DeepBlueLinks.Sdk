import Observation

// Stands for your app's own navigation. It only records where the app would go, so the screen can show it.
@MainActor
@Observable
final class Router {
    static let shared = Router()

    private(set) var path: String?
    private(set) var query: String?

    func open(path: String, query: String?) {
        self.path = path
        self.query = query
    }
}
