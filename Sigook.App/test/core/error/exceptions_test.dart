import 'package:flutter_test/flutter_test.dart';
import 'package:sigook_app_flutter/core/error/exceptions.dart';

void main() {
  group('ServerException', () {
    test('toString includes message and statusCode', () {
      final e = ServerException(message: 'Not found', statusCode: 404);
      expect(e.toString(), contains('Not found'));
      expect(e.toString(), contains('404'));
    });

    test('toString works with null statusCode', () {
      final e = ServerException(message: 'Oops', statusCode: null);
      expect(e.toString(), contains('Oops'));
      expect(e.toString(), contains('null'));
    });
  });

  group('NetworkException', () {
    test('has default message', () {
      final e = NetworkException();
      expect(e.message, isNotEmpty);
    });

    test('toString does not throw', () {
      final e = NetworkException('custom network msg');
      expect(e.toString(), contains('custom network msg'));
    });
  });

  group('CacheException', () {
    test('has default message', () {
      final e = CacheException();
      expect(e.message, isNotEmpty);
    });

    test('toString includes message', () {
      final e = CacheException('cache failed');
      expect(e.toString(), contains('cache failed'));
    });
  });

  group('ParseException', () {
    test('has default message', () {
      final e = ParseException();
      expect(e.message, isNotEmpty);
    });

    test('toString includes message', () {
      final e = ParseException('bad json');
      expect(e.toString(), contains('bad json'));
    });
  });
}
