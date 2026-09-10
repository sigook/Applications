import 'package:flutter_test/flutter_test.dart';
import 'package:sigook_app_flutter/features/auth/domain/entities/auth_token.dart';

void main() {
  group('AuthToken', () {
    test('hasAccessToken and hasRefreshToken require non-empty values', () {
      const empty = AuthToken(accessToken: '', refreshToken: '');
      const full = AuthToken(accessToken: 'access', refreshToken: 'refresh');

      expect(const AuthToken().hasAccessToken, false);
      expect(const AuthToken().hasRefreshToken, false);
      expect(empty.hasAccessToken, false);
      expect(empty.hasRefreshToken, false);
      expect(full.hasAccessToken, true);
      expect(full.hasRefreshToken, true);
    });

    test('isExpired is true without an expiration date', () {
      expect(const AuthToken(accessToken: 'access').isExpired(), true);
    });

    test('isExpired compares against now plus the leeway', () {
      final token = AuthToken(
        accessToken: 'access',
        expirationDateTime: DateTime.now().add(const Duration(seconds: 45)),
      );

      expect(token.isExpired(), false);
      expect(token.isExpired(leeway: const Duration(seconds: 30)), false);
      expect(token.isExpired(leeway: AuthToken.defaultExpiryLeeway), true);
    });

    test('isExpired is true once the expiration date has passed', () {
      final token = AuthToken(
        accessToken: 'access',
        expirationDateTime: DateTime.now().subtract(const Duration(seconds: 1)),
      );

      expect(token.isExpired(), true);
    });
  });
}
