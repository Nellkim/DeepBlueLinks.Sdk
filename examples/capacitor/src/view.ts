import { LINK_DOMAIN, URL_SCHEME } from './config'
import type { ResolvedLink } from './deepBlueLinks'

export type InstallLinkCheck = {
  trigger: 'first launch' | 'button'
  checkedOn: string
  link?: ResolvedLink | null
  error?: string
}

type Row = [term: string, value: string]

function byId(id: string): HTMLElement {
  const element = document.getElementById(id)
  if (!element) throw new Error(`index.html has no #${id}`)
  return element
}

function showRows(id: string, rows: Row[]) {
  byId(id).replaceChildren(
    ...rows.flatMap(([term, value]) => {
      const dt = document.createElement('dt')
      dt.textContent = term
      const dd = document.createElement('dd')
      dd.textContent = value
      return [dt, dd]
    }),
  )
}

function linkRows(link: ResolvedLink): Row[] {
  return [
    ['id', String(link.id)],
    ['longUrl', link.longUrl],
    ['shortUrl', link.shortUrl],
    ['isSandbox', link.isSandbox ? 'true' : 'absent: a live link'],
  ]
}

export function errorMessage(error: unknown): string {
  if (!(error instanceof Error)) return String(error)
  const code = (error as { code?: unknown }).code
  return typeof code === 'string' ? `${code}: ${error.message}` : error.message
}

export function showNoLinkYet() {
  showRows('opened-link', [
    ['Result', 'No link yet'],
    ['Try', `https://${LINK_DOMAIN}/<short code> or ${URL_SCHEME}://<path>`],
  ])
}

export function showOpenedLink(url: string, link: ResolvedLink | null) {
  const opened: Row = ['Opened with', url]
  if (link) {
    showRows('opened-link', [opened, ...linkRows(link)])
    return
  }
  const result = url.startsWith(`${URL_SCHEME}:`)
    ? 'The URL scheme: routed by its path, without calling the API'
    : "Not one of the app's short links (404): routed as it is"
  showRows('opened-link', [opened, ['Result', result]])
}

export function showCheckingInstallLink() {
  showRows('install-link', [['Result', 'Checking…']])
}

export function showInstallLink(check: InstallLinkCheck) {
  const when = check.trigger === 'first launch' ? 'on the first launch' : 'with the button'
  const checked: Row = ['Checked', `${new Date(check.checkedOn).toLocaleString()}, ${when}`]
  if (check.error) {
    showRows('install-link', [checked, ['Error', check.error]])
    return
  }
  const rows: Row[] = check.link ? linkRows(check.link) : [['Result', 'No deferred link']]
  showRows('install-link', [checked, ...rows])
}

export function showRoute(path: string) {
  byId('route').textContent = path
}

export function showError(error: unknown) {
  byId('last-error').textContent = `${new Date().toLocaleTimeString()}: ${errorMessage(error)}`
}
