import { App } from '@capacitor/app'
import { Capacitor } from '@capacitor/core'
import { InstallReferrer } from '@capgo/capacitor-install-referrer'
import { API_KEY, API_URL, URL_SCHEME } from './config'
import { router } from './router'
import { showError, showOpenedLink } from './view'

export type ResolvedLink = { id: number; longUrl: string; shortUrl: string; isSandbox?: boolean }

export async function post(path: string, body: unknown): Promise<ResolvedLink | null> {
  const response = await fetch(`${API_URL}${path}`, {
    method: 'POST',
    headers: { 'Api-Key': API_KEY, 'Content-Type': 'application/json' },
    body: JSON.stringify(body),
  })
  if (response.status === 404) return null
  if (!response.ok) throw new Error(`Deep Blue Links answered ${response.status}`)
  return response.json()
}

export const resolveLink = (url: string) => post('/api/links/resolve', { url })

let lastUrl: string | undefined

async function openUrl(url: string) {
  if (url === lastUrl) return
  lastUrl = url
  const opened = new URL(url)
  if (opened.protocol === `${URL_SCHEME}:`) {
    showOpenedLink(url, null)
    router.open('/' + opened.host + opened.pathname + opened.search)
    return
  }
  const link = await resolveLink(url)
  showOpenedLink(url, link)
  const destination = new URL(link?.longUrl ?? url)
  router.open(destination.pathname + destination.search)
}

export function listenForLinks() {
  App.addListener('appUrlOpen', ({ url }) => openUrl(url).catch(showError))
  App.getLaunchUrl().then((launch) => { if (launch?.url) return openUrl(launch.url) }).catch(showError)
}

export async function findInstallLink(): Promise<ResolvedLink | null> {
  if (Capacitor.getPlatform() === 'android') {
    const result = await InstallReferrer.getReferrer()
    const link = new URLSearchParams(result.referrer ?? '').get('link')
    return link ? resolveLink(link) : null
  }
  return post('/api/links/check-deferred', {
    timezone: Intl.DateTimeFormat().resolvedOptions().timeZone,
    device: {
      screenWidth: Math.round(screen.width * devicePixelRatio),
      screenHeight: Math.round(screen.height * devicePixelRatio),
      language: navigator.language,
    },
  })
}
