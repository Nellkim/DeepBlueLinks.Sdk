import UIKit

struct DeferredRequest: Encodable {
    struct Device: Encodable {
        let screenWidth: Double
        let screenHeight: Double
        let language: String
    }
    let timezone: String
    let device: Device?
}

extension DeepBlueLinks {
    @MainActor
    static func checkDeferred() async throws -> ResolvedLink? {
        let screen = (UIApplication.shared.connectedScenes.first as? UIWindowScene)?.screen
        let device = screen.map { screen in
            DeferredRequest.Device(
                screenWidth: Double(screen.bounds.width * screen.scale),
                screenHeight: Double(screen.bounds.height * screen.scale),
                language: Locale.preferredLanguages.first ?? "en")
        }
        let body = DeferredRequest(timezone: TimeZone.current.identifier, device: device)
        return try await post("\(Config.apiURL)/api/links/check-deferred", body: body)
    }
}
