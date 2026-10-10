import 'package:flutter_test/flutter_test.dart';
import 'package:sigook_app_flutter/core/error/failures.dart';

void main() {
  group('Failure equality (Equatable)', () {
    test('two ServerFailure with same message are equal', () {
      const a = ServerFailure(message: 'error');
      const b = ServerFailure(message: 'error');
      expect(a, equals(b));
    });

    test('two ServerFailure with different messages are not equal', () {
      const a = ServerFailure(message: 'error A');
      const b = ServerFailure(message: 'error B');
      expect(a, isNot(equals(b)));
    });

    test('ServerFailure and NetworkFailure with same message are not equal', () {
      const a = ServerFailure(message: 'error');
      const b = NetworkFailure(message: 'error');
      expect(a, isNot(equals(b)));
    });
  });

  group('Default messages', () {
    test('ServerFailure has default message', () {
      const f = ServerFailure();
      expect(f.message, isNotEmpty);
    });

    test('NetworkFailure has default message', () {
      const f = NetworkFailure();
      expect(f.message, isNotEmpty);
    });

    test('CacheFailure has default message', () {
      const f = CacheFailure();
      expect(f.message, isNotEmpty);
    });

    test('ParseFailure has default message', () {
      const f = ParseFailure();
      expect(f.message, isNotEmpty);
    });

    test('ValidationFailure has default message', () {
      const f = ValidationFailure();
      expect(f.message, isNotEmpty);
    });

    test('PermissionFailure has default message', () {
      const f = PermissionFailure();
      expect(f.message, isNotEmpty);
    });

    test('UserCancelledFailure has default message', () {
      const f = UserCancelledFailure();
      expect(f.message, isNotEmpty);
    });
  });

  group('Custom messages are preserved', () {
    test('ServerFailure preserves custom message', () {
      const f = ServerFailure(message: 'custom server error');
      expect(f.message, 'custom server error');
    });

    test('NetworkFailure preserves custom message', () {
      const f = NetworkFailure(message: 'custom network error');
      expect(f.message, 'custom network error');
    });
  });
}
