import 'dart:convert';

import 'package:flutter_test/flutter_test.dart';
import 'package:sigook_app_flutter/features/auth/data/models/auth_token_model.dart';

String fakeJwt(Map<String, dynamic> claims) {
  final header = base64Url.encode(utf8.encode(json.encode({'alg': 'none'})));
  final payload = base64Url.encode(utf8.encode(json.encode(claims)));
  return '$header.$payload.signature';
}

void main() {
  group('fromLoginResponse', () {
    test('maps the full assumed contract', () {
      final idToken = fakeJwt({
        'sub': 'user-1',
        'email': 'test@example.com',
        'role': 'worker',
      });

      final model = AuthTokenModel.fromLoginResponse({
        'accessToken': 'access-123',
        'idToken': idToken,
        'refreshToken': 'refresh-456',
        'expiresIn': 3600,
        'tokenType': 'Bearer',
        'scopes': ['openid', 'profile', 'offline_access'],
      });

      expect(model.accessToken, 'access-123');
      expect(model.idToken, idToken);
      expect(model.refreshToken, 'refresh-456');
      expect(model.tokenType, 'Bearer');
      expect(model.scopes, ['openid', 'profile', 'offline_access']);
      expect(model.expirationDateTime, isNotNull);
      expect(
        model.expirationDateTime!.isAfter(DateTime.now()),
        true,
      );
      expect(model.userInfo, isNotNull);
      expect(model.userInfo!.sub, 'user-1');
      expect(model.userInfo!.email, 'test@example.com');
    });

    test('prefers expirationDateTime over expiresIn when both present', () {
      final explicitExpiration =
          DateTime.now().add(const Duration(days: 2)).toUtc();

      final model = AuthTokenModel.fromLoginResponse({
        'accessToken': 'access-123',
        'expirationDateTime': explicitExpiration.toIso8601String(),
        'expiresIn': 60,
      });

      expect(model.expirationDateTime, explicitExpiration);
    });

    test('parses scope as a space-delimited string', () {
      final model = AuthTokenModel.fromLoginResponse({
        'accessToken': 'access-123',
        'scope': 'openid profile offline_access',
      });

      expect(model.scopes, ['openid', 'profile', 'offline_access']);
    });

    test('defaults tokenType to Bearer and leaves missing fields null', () {
      final model = AuthTokenModel.fromLoginResponse({
        'accessToken': 'access-123',
      });

      expect(model.accessToken, 'access-123');
      expect(model.tokenType, 'Bearer');
      expect(model.idToken, isNull);
      expect(model.refreshToken, isNull);
      expect(model.expirationDateTime, isNull);
      expect(model.scopes, isNull);
      expect(model.userInfo, isNull);
    });

    test('returns null userInfo when idToken is not a decodable JWT', () {
      final model = AuthTokenModel.fromLoginResponse({
        'accessToken': 'access-123',
        'idToken': 'not-a-jwt',
      });

      expect(model.accessToken, 'access-123');
      expect(model.idToken, 'not-a-jwt');
      expect(model.userInfo, isNull);
    });
  });
}
