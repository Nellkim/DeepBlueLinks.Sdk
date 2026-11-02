import Foundation
import Observation

@MainActor
@Observable
final class AppModel {
    enum OpenResult {
        case resolving
        case resolved(ResolvedLink)
        case notALink
        case urlScheme
        case failed
    }

    enum DeferredResult {
        case checking
        case found(ResolvedLink)
        case noLink
        case failed(String)
        case checkedOnEarlierLaunch
    }

    private(set) var openedURL: URL?
    private(set) var openResult: OpenResult?
    private(set) var deferredResult: DeferredResult?
    private(set) var lastError: String?

    var isCheckingDeferredLink: Bool {
        if case .checking = deferredResult { true } else { false }
    }

    func open(_ url: URL) async {
        openedURL = url

        if url.scheme == Config.urlScheme {
            openResult = .urlScheme
            Router.shared.open(path: "/" + (url.host ?? "") + url.path, query: url.query)
            return
        }

        openResult = .resolving
        let link: ResolvedLink?
        do {
            link = try await DeepBlueLinks.resolve(url)
            openResult = if let link { .resolved(link) } else { .notALink }
        } catch {
            link = nil
            openResult = .failed
            lastError = "POST /api/links/resolve: \(error.localizedDescription)"
        }

        let destination = link.flatMap { URL(string: $0.longUrl) } ?? url
        Router.shared.open(path: destination.path, query: destination.query)
    }

    func checkDeferredLinkOnFirstLaunch() async {
        if !UserDefaults.standard.bool(forKey: "checkedDeferredLink") {
            UserDefaults.standard.set(true, forKey: "checkedDeferredLink")
            await checkDeferredLink()
        } else if deferredResult == nil {
            deferredResult = .checkedOnEarlierLaunch
        }
    }

    func checkDeferredLink() async {
        deferredResult = .checking
        do {
            if let link = try await DeepBlueLinks.checkDeferred() {
                deferredResult = .found(link)
                if let destination = URL(string: link.longUrl) {
                    Router.shared.open(path: destination.path, query: destination.query)
                }
            } else {
                deferredResult = .noLink
            }
        } catch {
            deferredResult = .failed(error.localizedDescription)
            lastError = "POST /api/links/check-deferred: \(error.localizedDescription)"
        }
    }
}
