import { useCallback, useEffect, useState } from 'react'
import { Button, Linking, ScrollView, StatusBar, StyleSheet, Text, View } from 'react-native'
import AsyncStorage from '@react-native-async-storage/async-storage'
import { SafeAreaProvider, SafeAreaView } from 'react-native-safe-area-context'
import { URL_SCHEME } from './config'
import { findInstallLink, pathOf, resolveLink, type ResolvedLink } from './deepBlueLinks'

const INSTALL_LINK_CHECKED = 'installLinkChecked'

type Opened = { url: string; route: string; link: ResolvedLink | null }

type InstallCheck =
  | { status: 'checking' }
  | { status: 'checkedEarlier' }
  | { status: 'done'; link: ResolvedLink | null }
  | { status: 'failed'; message: string }

// An app passes the route to its navigation, router.open(route) in the guide; this example shows it.
async function openUrl(url: string): Promise<Opened> {
  if (url.startsWith(`${URL_SCHEME}://`)) {
    return { url, route: '/' + url.slice(`${URL_SCHEME}://`.length), link: null }
  }
  const link = await resolveLink(url)
  return { url, route: pathOf(link?.longUrl ?? url), link }
}

const messageOf = (error: unknown) => (error instanceof Error ? error.message : String(error))

export default function App() {
  const [opened, setOpened] = useState<Opened | null>(null)
  const [installCheck, setInstallCheck] = useState<InstallCheck>({ status: 'checking' })
  const [lastError, setLastError] = useState<string | null>(null)

  useEffect(() => {
    const open = (url: string) => openUrl(url).then(setOpened, (error) => setLastError(messageOf(error)))
    Linking.getInitialURL().then((url) => {
      if (url) open(url)
    })
    const subscription = Linking.addEventListener('url', ({ url }) => open(url))
    return () => subscription.remove()
  }, [])

  const checkInstallLink = useCallback(async () => {
    setInstallCheck({ status: 'checking' })
    const link = await findInstallLink()
    setInstallCheck({ status: 'done', link })
  }, [])

  const failInstallCheck = useCallback((error: unknown) => {
    setInstallCheck({ status: 'failed', message: messageOf(error) })
    setLastError(messageOf(error))
  }, [])

  useEffect(() => {
    async function checkOnFirstLaunch() {
      if (await AsyncStorage.getItem(INSTALL_LINK_CHECKED)) {
        setInstallCheck({ status: 'checkedEarlier' })
        return
      }
      await checkInstallLink()
      // Stored once the lookup completes, so a lookup that fails runs again on the next launch.
      await AsyncStorage.setItem(INSTALL_LINK_CHECKED, 'true')
    }
    checkOnFirstLaunch().catch(failInstallCheck)
  }, [checkInstallLink, failInstallCheck])

  return (
    <SafeAreaProvider>
      <StatusBar barStyle="dark-content" />
      <SafeAreaView style={styles.screen}>
        <ScrollView contentContainerStyle={styles.content}>
          <Text style={styles.title}>Deep Blue Links example</Text>

          <Text style={styles.heading}>Link that opened the app</Text>
          {opened ? <OpenedLink opened={opened} /> : <Text style={styles.text}>No link yet</Text>}

          <Text style={styles.heading}>Deferred link from the first launch</Text>
          <InstallLink check={installCheck} />
          <View style={styles.button}>
            <Button
              title="Check for a deferred link"
              disabled={installCheck.status === 'checking'}
              onPress={() => checkInstallLink().catch(failInstallCheck)}
            />
          </View>
          <Text style={styles.note}>Testing aid: a real app runs this lookup only once, on its first launch.</Text>

          {lastError ? (
            <>
              <Text style={styles.heading}>Last error</Text>
              <Text style={styles.error}>{lastError}</Text>
            </>
          ) : null}
        </ScrollView>
      </SafeAreaView>
    </SafeAreaProvider>
  )
}

function OpenedLink({ opened }: { opened: Opened }) {
  return (
    <>
      <Field name="url" value={opened.url} />
      {opened.link ? (
        <LinkFields link={opened.link} />
      ) : (
        <Text style={styles.text}>
          {opened.url.startsWith(`${URL_SCHEME}://`)
            ? 'Opened with the URL scheme, which carries the path: not resolved.'
            : 'Not one of the app’s short links (404): routed as it is.'}
        </Text>
      )}
      <Field name="route" value={opened.route} />
    </>
  )
}

function InstallLink({ check }: { check: InstallCheck }) {
  switch (check.status) {
    case 'checking':
      return <Text style={styles.text}>Checking…</Text>
    case 'checkedEarlier':
      return <Text style={styles.text}>Checked on an earlier launch</Text>
    case 'failed':
      return <Text style={styles.error}>{check.message}</Text>
    case 'done':
      return check.link ? (
        <>
          <LinkFields link={check.link} />
          <Field name="route" value={pathOf(check.link.longUrl)} />
        </>
      ) : (
        <Text style={styles.text}>No deferred link</Text>
      )
  }
}

function LinkFields({ link }: { link: ResolvedLink }) {
  return (
    <>
      <Field name="id" value={String(link.id)} />
      <Field name="longUrl" value={link.longUrl} />
      <Field name="shortUrl" value={link.shortUrl} />
      <Field name="isSandbox" value={String(link.isSandbox ?? false)} />
    </>
  )
}

function Field({ name, value }: { name: string; value: string }) {
  return (
    <Text style={styles.text}>
      <Text style={styles.name}>{name}: </Text>
      {value}
    </Text>
  )
}

const styles = StyleSheet.create({
  screen: { flex: 1, backgroundColor: '#ffffff' },
  content: { padding: 20 },
  title: { fontSize: 22, fontWeight: '700', color: '#111111' },
  heading: { marginTop: 24, marginBottom: 6, fontSize: 16, fontWeight: '600', color: '#111111' },
  text: { marginBottom: 4, fontSize: 14, color: '#222222' },
  name: { fontWeight: '600' },
  button: { marginTop: 12 },
  note: { marginTop: 6, fontSize: 13, color: '#555555' },
  error: { marginBottom: 4, fontSize: 14, color: '#b00020' },
})
