import SwiftUI

struct ContentView: View {
    let model: AppModel

    var body: some View {
        NavigationStack {
            List {
                Section("Link that opened the app") {
                    if let url = model.openedURL {
                        ValueRow("URL", url.absoluteString)
                        openResult
                    } else {
                        Text("No link yet")
                        Text("Open a link on \(Config.linkDomain) or a \(Config.urlScheme):// URL.")
                            .font(.footnote)
                            .foregroundStyle(.secondary)
                    }
                }

                Section {
                    deferredResult
                    Button("Check for a deferred link") {
                        Task {
                            await model.checkDeferredLink()
                        }
                    }
                    .disabled(model.isCheckingDeferredLink)
                } header: {
                    Text("Deferred link")
                } footer: {
                    Text("The button is a testing aid. A real app checks once, on its first launch.")
                }

                Section {
                    if let path = Router.shared.path {
                        ValueRow("path", path)
                        ValueRow("query", Router.shared.query ?? "(none)")
                    } else {
                        Text("Nothing opened yet")
                    }
                } header: {
                    Text("Route")
                } footer: {
                    Text("Where the app navigates. Router stands for your app's own navigation.")
                }

                if let lastError = model.lastError {
                    Section("Last error") {
                        Text(lastError)
                            .foregroundStyle(.red)
                    }
                }
            }
            .navigationTitle("Deep Blue Links example")
            .navigationBarTitleDisplayMode(.inline)
        }
    }

    @ViewBuilder
    private var openResult: some View {
        switch model.openResult {
        case nil, .resolving:
            Text("Resolving…")
                .foregroundStyle(.secondary)
        case .resolved(let link):
            LinkRows(link: link)
        case .notALink:
            Text("Not one of your short links (404), so the app routes by the URL itself.")
        case .urlScheme:
            Text("Opened through the URL scheme. It isn't a short link, so it isn't resolved.")
        case .failed:
            Text("Not resolved, so the app routes by the URL itself. See the last error.")
        }
    }

    @ViewBuilder
    private var deferredResult: some View {
        switch model.deferredResult {
        case nil:
            Text("Not checked yet")
                .foregroundStyle(.secondary)
        case .checking:
            Text("Checking…")
                .foregroundStyle(.secondary)
        case .found(let link):
            LinkRows(link: link)
        case .noLink:
            Text("No deferred link")
        case .failed(let message):
            Text(message)
                .foregroundStyle(.red)
        case .checkedOnEarlierLaunch:
            Text("Checked on an earlier launch")
        }
    }
}

private struct LinkRows: View {
    let link: ResolvedLink

    var body: some View {
        ValueRow("id", String(link.id))
        ValueRow("longUrl", link.longUrl)
        ValueRow("shortUrl", link.shortUrl)
        ValueRow("isSandbox", link.isSandbox == true ? "true" : "false")
    }
}

private struct ValueRow: View {
    let label: String
    let value: String

    init(_ label: String, _ value: String) {
        self.label = label
        self.value = value
    }

    var body: some View {
        VStack(alignment: .leading, spacing: 2) {
            Text(label)
                .font(.caption)
                .foregroundStyle(.secondary)
            Text(value)
                .textSelection(.enabled)
        }
    }
}
