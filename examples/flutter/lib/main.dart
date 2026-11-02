import 'dart:async';

import 'package:app_links/app_links.dart';
import 'package:flutter/material.dart';
import 'package:shared_preferences/shared_preferences.dart';

import 'config.dart';
import 'deep_blue_links.dart';

void main() => runApp(const ExampleApp());

typedef OpenedLink = ({Uri url, String route, ResolvedLink? link});

class ExampleApp extends StatefulWidget {
  const ExampleApp({super.key});

  @override
  State<ExampleApp> createState() => _ExampleAppState();
}

class _ExampleAppState extends State<ExampleApp> {
  static const _installLinkCheckedKey = 'installLinkChecked';

  final _appLinks = AppLinks();
  StreamSubscription<Uri>? _linkSubscription;

  OpenedLink? _opened;
  ResolvedLink? _installLink;
  String _installLinkStatus = '';
  bool _checkingInstallLink = false;
  String? _lastError;

  @override
  void initState() {
    super.initState();
    listenForLinks();
    _findInstallLinkOnFirstLaunch();
  }

  @override
  void dispose() {
    _linkSubscription?.cancel();
    super.dispose();
  }

  void listenForLinks() {
    _linkSubscription = _appLinks.uriLinkStream.listen((uri) async {
      try {
        if (uri.scheme == urlScheme) {
          _open(uri, '/${uri.host}${uri.path}', uri.query);
          return;
        }
        final link = await DeepBlueLinks.resolve(uri);
        final destination = link == null ? uri : Uri.parse(link.longUrl);
        _open(uri, destination.path, destination.query, link);
      } catch (error) {
        setState(() => _lastError = 'Resolving $uri failed: $error');
      }
    });
  }

  // Stands in for the guide's router.open: the example shows the route
  // instead of navigating to it.
  void _open(Uri url, String path, String query, [ResolvedLink? link]) {
    setState(() {
      _opened = (url: url, route: query.isEmpty ? path : '$path?$query', link: link);
    });
  }

  Future<void> _findInstallLinkOnFirstLaunch() async {
    final preferences = SharedPreferencesAsync();
    if (await preferences.getBool(_installLinkCheckedKey) ?? false) {
      setState(() => _installLinkStatus = 'Checked on an earlier launch');
      return;
    }
    await _findInstallLink();
    await preferences.setBool(_installLinkCheckedKey, true);
  }

  Future<void> _findInstallLink() async {
    setState(() {
      _checkingInstallLink = true;
      _installLink = null;
      _installLinkStatus = 'Checking…';
    });
    try {
      final link = await DeepBlueLinks.findInstallLink();
      setState(() {
        _installLink = link;
        _installLinkStatus = link == null ? 'No deferred link' : '';
      });
    } catch (error) {
      setState(() {
        _installLinkStatus = 'Error: $error';
        _lastError = 'Finding the deferred link failed: $error';
      });
    } finally {
      setState(() => _checkingInstallLink = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    final opened = _opened;
    final installLink = _installLink;
    final lastError = _lastError;

    return MaterialApp(
      title: 'Deep Blue Links example',
      home: Scaffold(
        appBar: AppBar(title: const Text('Deep Blue Links example')),
        body: ListView(
          padding: const EdgeInsets.all(16),
          children: [
            const _Heading('Link that opened the app'),
            if (opened == null)
              const Text('No link yet')
            else ...[
              SelectableText('url: ${opened.url}\nroute: ${opened.route}'),
              if (opened.link case final link?)
                _LinkDetails(link)
              else if (opened.url.scheme == urlScheme)
                const Text('URL scheme: routed without calling the API')
              else
                const Text('Not a short link (resolve answered 404): routed as it is'),
            ],
            const _Heading('Deferred link'),
            if (installLink != null) _LinkDetails(installLink) else Text(_installLinkStatus),
            const SizedBox(height: 12),
            FilledButton(
              onPressed: _checkingInstallLink ? null : _findInstallLink,
              child: const Text('Check for a deferred link'),
            ),
            const SizedBox(height: 4),
            const Text('Testing aid: a real app runs this lookup only once, on its first launch.'),
            if (lastError != null) ...[const _Heading('Last error'), SelectableText(lastError)],
          ],
        ),
      ),
    );
  }
}

class _Heading extends StatelessWidget {
  const _Heading(this.text);

  final String text;

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.only(top: 16, bottom: 4),
      child: Text(text, style: Theme.of(context).textTheme.titleMedium),
    );
  }
}

class _LinkDetails extends StatelessWidget {
  const _LinkDetails(this.link);

  final ResolvedLink link;

  @override
  Widget build(BuildContext context) {
    return SelectableText(
      'id: ${link.id}\n'
      'longUrl: ${link.longUrl}\n'
      'shortUrl: ${link.shortUrl}\n'
      'isSandbox: ${link.isSandbox}',
    );
  }
}
