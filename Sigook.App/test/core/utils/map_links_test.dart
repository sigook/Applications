import 'package:flutter_test/flutter_test.dart';
import 'package:sigook_app_flutter/core/utils/map_links.dart';

void main() {
  group('mapsSearchUri', () {
    test('builds the Google Maps search url for the coordinates', () {
      final uri = mapsSearchUri(45.4215, -75.6972);

      expect(uri.host, 'www.google.com');
      expect(uri.path, '/maps/search/');
      expect(uri.queryParameters['api'], '1');
      expect(uri.queryParameters['query'], '45.4215,-75.6972');
    });
  });

  group('staticMapUri', () {
    test('returns null without an api key', () {
      expect(staticMapUri(45.4215, -75.6972, apiKey: ''), isNull);
    });

    test('centers and marks the coordinates', () {
      final uri = staticMapUri(45.4215, -75.6972, apiKey: 'key')!;

      expect(uri.host, 'maps.googleapis.com');
      expect(uri.path, '/maps/api/staticmap');
      expect(uri.queryParameters['center'], '45.4215,-75.6972');
      expect(uri.queryParameters['markers'], 'color:red|45.4215,-75.6972');
      expect(uri.queryParameters['size'], '640x320');
      expect(uri.queryParameters['scale'], '2');
      expect(uri.queryParameters['key'], 'key');
    });
  });
}
