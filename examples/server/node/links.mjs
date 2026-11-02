// Creates a link, updates it and deletes it again with Deep Blue Links' API.
// Needs DBL_API_KEY and DBL_APP_ID; DBL_API_URL defaults to https://thedbl.ink.

const apiUrl = process.env.DBL_API_URL ?? 'https://thedbl.ink'
const apiKey = process.env.DBL_API_KEY
const appId = Number(process.env.DBL_APP_ID)
if (!apiKey || !appId) {
  console.error("Set DBL_API_KEY to your app's API key (the sandbox key outside production) and DBL_APP_ID to its id.")
  process.exit(1)
}

async function call(method, path, body) {
  const response = await fetch(`${apiUrl}${path}`, {
    method,
    headers: { 'Api-Key': apiKey, ...(body ? { 'Content-Type': 'application/json' } : {}) },
    body: body ? JSON.stringify(body) : undefined,
  })
  const text = await response.text()
  if (!response.ok) throw new Error(`Deep Blue Links answered ${response.status}: ${text || '(empty body)'}`)
  return text ? JSON.parse(text) : null
}

const link = await call('POST', '/api/links', {
  appId,
  fullUrl: '/products/1234',
  title: 'Spring sale',
  description: 'Up to 50% off this week only.',
  imageUrl: 'https://example.com/images/spring-sale.png',
  fallbackUrl: 'https://example.com/spring-sale',
})
console.log(`Created link ${link.id}: ${link.shortUrl}`)

const updated = await call('PUT', `/api/links/${link.id}`, {
  appId,
  fullUrl: '/products/1234',
  title: 'Spring sale, extended',
  description: '',
  imageUrl: '',
  fallbackUrl: '',
})
console.log(`Updated its title to "${updated.title}"`)

await call('DELETE', `/api/links/${link.id}`)
console.log(`Deleted link ${link.id}`)
