import Foundation

struct ResolvedLink: Decodable {
    let id: Int
    let longUrl: String
    let shortUrl: String
    let isSandbox: Bool?
}

struct DeepBlueLinksError: LocalizedError {
    let status: Int
    var errorDescription: String? { "Deep Blue Links answered \(status)" }
}

enum DeepBlueLinks {
    static let apiKey = Config.apiKey

    static func resolve(_ url: URL) async throws -> ResolvedLink? {
        try await post("\(Config.apiURL)/api/links/resolve", body: ["url": url.absoluteString])
    }

    static func post<Body: Encodable>(_ endpoint: String, body: Body) async throws -> ResolvedLink? {
        var request = URLRequest(url: URL(string: endpoint)!)
        request.httpMethod = "POST"
        request.setValue(apiKey, forHTTPHeaderField: "Api-Key")
        request.setValue("application/json", forHTTPHeaderField: "Content-Type")
        request.httpBody = try JSONEncoder().encode(body)

        let (data, response) = try await URLSession.shared.data(for: request)
        let status = (response as? HTTPURLResponse)?.statusCode ?? 0
        if status == 404 { return nil }
        guard status == 200 else { throw DeepBlueLinksError(status: status) }
        return try JSONDecoder().decode(ResolvedLink.self, from: data)
    }
}
