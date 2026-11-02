import 'dart:convert';
import 'dart:io' show Platform;
import 'dart:ui' show PlatformDispatcher;

import 'package:flutter_timezone/flutter_timezone.dart';
import 'package:http/http.dart' as http;
import 'package:play_install_referrer/play_install_referrer.dart';

import 'config.dart';

class ResolvedLink {
  ResolvedLink.fromJson(Map<String, dynamic> json)
    : id = json['id'] as int,
      longUrl = json['longUrl'] as String,
      shortUrl = json['shortUrl'] as String,
      isSandbox = json['isSandbox'] == true;

  final int id;
  final String longUrl;
  final String shortUrl;
  final bool isSandbox;
}

class DeepBlueLinks {
  static Future<ResolvedLink?> resolve(Uri url) =>
      _post('/api/links/resolve', {'url': url.toString()});

  static Future<ResolvedLink?> _post(String path, Map<String, Object?> body) async {
    final response = await http.post(
      Uri.parse('$apiUrl$path'),
      headers: {'Api-Key': apiKey, 'Content-Type': 'application/json'},
      body: jsonEncode(body),
    );
    if (response.statusCode == 404) return null;
    if (response.statusCode != 200) {
      throw http.ClientException('Deep Blue Links answered ${response.statusCode}');
    }
    return ResolvedLink.fromJson(jsonDecode(response.body) as Map<String, dynamic>);
  }

  static Future<ResolvedLink?> findInstallLink() async {
    if (Platform.isAndroid) {
      final String? link;
      try {
        final details = await PlayInstallReferrer.installReferrer;
        link = Uri.splitQueryString(details.installReferrer ?? '')['link'];
      } catch (_) {
        return null;
      }
      return link == null ? null : resolve(Uri.parse(link));
    }

    final timezone = await FlutterTimezone.getLocalTimezone();
    final screen = PlatformDispatcher.instance.views.first.physicalSize;
    return _post('/api/links/check-deferred', {
      'timezone': timezone.identifier,
      'device': {
        'screenWidth': screen.width,
        'screenHeight': screen.height,
        'language': PlatformDispatcher.instance.locale.toLanguageTag(),
      },
    });
  }
}
