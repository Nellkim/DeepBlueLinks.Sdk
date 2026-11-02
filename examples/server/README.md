# Server examples

Your backend creates the links your app shares: a product, an invitation, a post. Each example here does the same three calls with the app's API key: it creates a link with [`POST /api/links`](https://deepbluelinks.io/docs/api/create-link), updates its title with [`PUT /api/links/{id}`](https://deepbluelinks.io/docs/api/update-link) and deletes it with [`DELETE /api/links/{id}`](https://deepbluelinks.io/docs/api/delete-link), printing each result. An error stops it with the status and the API's message.

| Folder | Language | Requirements |
|---|---|---|
| [`curl/`](curl) | Bash and curl | Bash and curl, as on macOS and most Linux distributions |
| [`node/`](node) | JavaScript | Node.js 18 or later, no packages |
| [`python/`](python) | Python | Python 3.9 or later and `requests` |
| [`dotnet/`](dotnet) | C# | .NET 10 SDK |

## Configure

Every example reads three environment variables:

| Variable | Value |
|---|---|
| `DBL_API_KEY` | Your app's API key, from its page in the [dashboard](https://app.deepbluelinks.io). Use the sandbox key, which starts with `sandbox_`, outside production: links it creates are marked as sandbox links, and it can never change a live one. |
| `DBL_APP_ID` | Your app's id, shown on the same page. |
| `DBL_API_URL` | Optional. Defaults to `https://thedbl.ink`. |

Keep the key in your secrets manager or environment, never in a web page or a repository.

## Run

```bash
export DBL_API_KEY=sandbox_...
export DBL_APP_ID=42

./curl/links.sh
node node/links.mjs
pip install -r python/requirements.txt && python python/links.py
dotnet run --project dotnet
```

Each run creates one link and deletes it again, so it leaves nothing behind, but the link still counts toward your plan's links for the month.

## Errors to expect

- **400** with `{ "message": ... }` when a field is missing, a short code is taken or the organization's links for the month are used up.
- **401** with an empty body when the key is wrong or was regenerated.

The [server guide](https://deepbluelinks.io/docs/guides/server) explains each call, custom short codes and short URLs on your own domain.
