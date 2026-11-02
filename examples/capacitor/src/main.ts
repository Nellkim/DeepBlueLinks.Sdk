import './style.css'
import { findInstallLink, listenForLinks } from './deepBlueLinks'
import { router } from './router'
import {
  errorMessage,
  showCheckingInstallLink,
  showError,
  showInstallLink,
  showNoLinkYet,
  type InstallLinkCheck,
} from './view'

const FIRST_LAUNCH_CHECK = 'deepBlueLinks.firstLaunchCheck'

async function checkInstallLink(trigger: InstallLinkCheck['trigger']): Promise<InstallLinkCheck> {
  const checkedOn = new Date().toISOString()
  try {
    const link = await findInstallLink()
    if (link) {
      const destination = new URL(link.longUrl)
      router.open(destination.pathname + destination.search)
    }
    return { trigger, checkedOn, link }
  } catch (error) {
    showError(error)
    return { trigger, checkedOn, error: errorMessage(error) }
  }
}

showNoLinkYet()
listenForLinks()

const button = document.querySelector<HTMLButtonElement>('#check-install-link')!

async function runCheck(trigger: InstallLinkCheck['trigger']) {
  button.disabled = true
  showCheckingInstallLink()
  const check = await checkInstallLink(trigger)
  button.disabled = false
  showInstallLink(check)
  return check
}

const firstLaunchCheck = localStorage.getItem(FIRST_LAUNCH_CHECK)
if (firstLaunchCheck) {
  showInstallLink(JSON.parse(firstLaunchCheck))
} else {
  runCheck('first launch').then((check) => localStorage.setItem(FIRST_LAUNCH_CHECK, JSON.stringify(check)))
}

button.addEventListener('click', () => runCheck('button'))
