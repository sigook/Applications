import 'package:flutter_test/flutter_test.dart';
import 'package:sigook_app_flutter/features/auth/data/models/auth_token_model.dart';

void main() {
  group('fromTokenResponse', () {
    test('maps the full password-grant response', () {
      final model = AuthTokenModel.fromTokenResponse({
        'access_token': 'access-123',
        'refresh_token': 'refresh-456',
        'expires_in': 3600,
        'token_type': 'Bearer',
        'scope': 'openid profile api1 roles offline_access',
      });

      expect(model.accessToken, 'access-123');
      expect(model.refreshToken, 'refresh-456');
      expect(model.tokenType, 'Bearer');
      expect(model.scopes, [
        'openid',
        'profile',
        'api1',
        'roles',
        'offline_access',
      ]);
      expect(model.expirationDateTime, isNotNull);
      expect(model.expirationDateTime!.isAfter(DateTime.now()), true);
      expect(model.idToken, isNull);
      expect(model.userInfo, isNull);
    });

    test('derives expirationDateTime from expires_in seconds', () {
      final before = DateTime.now().add(const Duration(seconds: 3599));

      final model = AuthTokenModel.fromTokenResponse({
        'access_token': 'access-123',
        'expires_in': 3600,
      });

      expect(model.expirationDateTime, isNotNull);
      expect(model.expirationDateTime!.isAfter(before), true);
    });

    test('defaults tokenType to Bearer and leaves missing fields null', () {
      final model = AuthTokenModel.fromTokenResponse({
        'access_token': 'access-123',
      });

      expect(model.accessToken, 'access-123');
      expect(model.tokenType, 'Bearer');
      expect(model.refreshToken, isNull);
      expect(model.expirationDateTime, isNull);
      expect(model.scopes, isNull);
      expect(model.idToken, isNull);
      expect(model.userInfo, isNull);
    });
  });
}
