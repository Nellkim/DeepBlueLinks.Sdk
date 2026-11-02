import { Dimensions, PixelRatio, Platform } from 'react-native'
import DeviceInfo from 'react-native-device-info'
import { API_KEY, API_URL } from './config'

export type ResolvedLink = { id: number; longUrl: string; shortUrl: string; isSandbox?: boolean }

async function post(path: string, body: unknown): Promise<ResolvedLink | null> {
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

/** The path and query of an https URL, such as /products/1234?ref=mail. */
export const pathOf = (url: string) => url.replace(/^https?:\/\/[^/?#]+/i, '') || '/'

function linkFromReferrer(referrer: string): string | null {
  for (const pair of referrer.split('&')) {
    const at = pair.indexOf('=')
    if (at > 0 && pair.slice(0, at) === 'link') return decodeURIComponent(pair.slice(at + 1))
  }
  return null
}

export async function findInstallLink(): Promise<ResolvedLink | null> {
  if (Platform.OS === 'android') {
    const referrer = await DeviceInfo.getInstallReferrer()
    if (referrer !== 'unknown') {
      const link = linkFromReferrer(referrer)
      return link ? resolveLink(link) : null
    }
    const { timeZone } = Intl.DateTimeFormat().resolvedOptions()
    return post('/api/links/check-deferred', { timezone: timeZone })
  }
  const { width, height } = Dimensions.get('screen')
  const ratio = PixelRatio.get()
  const { timeZone, locale } = Intl.DateTimeFormat().resolvedOptions()
  return post('/api/links/check-deferred', {
    timezone: timeZone,
    device: { screenWidth: Math.round(width * ratio), screenHeight: Math.round(height * ratio), language: locale },
  })
}
