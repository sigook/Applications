import 'dart:convert';

import 'package:dio/dio.dart';
import 'package:flutter_appauth/flutter_appauth.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:mocktail/mocktail.dart';
import 'package:sigook_app_flutter/core/constants/error_messages.dart';
import 'package:sigook_app_flutter/core/error/exceptions.dart';
import 'package:sigook_app_flutter/features/auth/data/datasources/auth_remote_datasource.dart';

import '../../../../helpers/mocks.dart';

class MockDio extends Mock implements Dio {}

class MockFlutterAppAuth extends Mock implements FlutterAppAuth {}

void main() {
  late MockDio mockDio;
  late MockDio mockAnonymousDio;
  late MockNetworkInfo mockNetwork;
  late AuthRemoteDataSourceImpl datasource;

  const tEmail = 'test@example.com';
  const tPassword = 'password123';

  final tRequestOptions = RequestOptions(path: '/connect/token');

  DioException badResponse(int statusCode, [dynamic data]) => DioException(
        requestOptions: tRequestOptions,
        response: Response(
          requestOptions: tRequestOptions,
          statusCode: statusCode,
          data: data,
        ),
        type: DioExceptionType.badResponse,
      );

  setUp(() {
    mockDio = MockDio();
    mockAnonymousDio = MockDio();
    mockNetwork = MockNetworkInfo();
    datasource = AuthRemoteDataSourceImpl(
      dio: mockDio,
      anonymousDio: mockAnonymousDio,
      networkInfo: mockNetwork,
      appAuth: MockFlutterAppAuth(),
    );
  });

  group('signIn', () {
    void stubTokenSuccess() {
      when(() => mockAnonymousDio.post(
            any(),
            data: any(named: 'data'),
            options: any(named: 'options'),
          )).thenAnswer(
        (_) async => Response(
          requestOptions: tRequestOptions,
          statusCode: 200,
          data: {
            'access_token': 'access-123',
            'refresh_token': 'refresh-456',
            'token_type': 'Bearer',
            'expires_in': 3600,
            'scope': 'openid profile api1 roles offline_access',
          },
        ),
      );
    }

    test('sends a form-urlencoded password grant and maps the response',
        () async {
      when(() => mockNetwork.isConnected).thenAnswer((_) async => true);
      stubTokenSuccess();

      final result = await datasource.signIn(
        email: tEmail,
        password: tPassword,
      );

      expect(result.accessToken, 'access-123');
      expect(result.refreshToken, 'refresh-456');
      expect(result.tokenType, 'Bearer');
      expect(result.expirationDateTime, isNotNull);
      expect(result.expirationDateTime!.isAfter(DateTime.now()), true);
      expect(result.scopes, contains('offline_access'));
      expect(result.idToken, isNull);
      expect(result.userInfo, isNull);

      final captured = verify(() => mockAnonymousDio.post(
            captureAny(),
            data: captureAny(named: 'data'),
            options: captureAny(named: 'options'),
          )).captured;
      expect(captured[0] as String, endsWith('/connect/token'));
      final body = captured[1] as Map;
      expect(body['grant_type'], 'password');
      expect(body['username'], tEmail);
      expect(body['password'], tPassword);
      expect(
        (captured[2] as Options).contentType,
        Headers.formUrlEncodedContentType,
      );
    });

    for (final entry in {
      'invalid_credentials': ErrorMessages.invalidCredentials,
      'inactive_user': ErrorMessages.inactiveUser,
      'email_not_confirmed': ErrorMessages.emailNotConfirmed,
      'locked_out': ErrorMessages.lockedOut,
    }.entries) {
      test('maps error_description ${entry.key} on 400', () async {
        when(() => mockNetwork.isConnected).thenAnswer((_) async => true);
        when(() => mockAnonymousDio.post(
              any(),
              data: any(named: 'data'),
              options: any(named: 'options'),
            )).thenThrow(badResponse(400, {
          'error': 'invalid_grant',
          'error_description': entry.key,
        }));

        await expectLater(
          () => datasource.signIn(email: tEmail, password: tPassword),
          throwsA(
            isA<ServerException>()
                .having((e) => e.message, 'message', entry.value)
                .having((e) => e.code, 'code', entry.key)
                .having((e) => e.statusCode, 'statusCode', 400),
          ),
        );
      });
    }

    test('falls back to invalid credentials on 400 without error_description',
        () async {
      when(() => mockNetwork.isConnected).thenAnswer((_) async => true);
      when(() => mockAnonymousDio.post(
            any(),
            data: any(named: 'data'),
            options: any(named: 'options'),
          )).thenThrow(badResponse(400, {'error': 'invalid_request'}));

      await expectLater(
        () => datasource.signIn(email: tEmail, password: tPassword),
        throwsA(
          isA<ServerException>()
              .having(
                (e) => e.message,
                'message',
                ErrorMessages.invalidCredentials,
              )
              .having((e) => e.statusCode, 'statusCode', 400),
        ),
      );
    });

    test('throws NetworkException without calling the endpoint when offline',
        () async {
      when(() => mockNetwork.isConnected).thenAnswer((_) async => false);

      await expectLater(
        () => datasource.signIn(email: tEmail, password: tPassword),
        throwsA(isA<NetworkException>()),
      );
      verifyNever(() => mockAnonymousDio.post(
            any(),
            data: any(named: 'data'),
            options: any(named: 'options'),
          ));
    });

    test('throws NetworkException on connection timeout', () async {
      when(() => mockNetwork.isConnected).thenAnswer((_) async => true);
      when(() => mockAnonymousDio.post(
            any(),
            data: any(named: 'data'),
            options: any(named: 'options'),
          )).thenThrow(
        DioException(
          requestOptions: tRequestOptions,
          type: DioExceptionType.connectionTimeout,
        ),
      );

      await expectLater(
        () => datasource.signIn(email: tEmail, password: tPassword),
        throwsA(isA<NetworkException>()),
      );
    });
  });

  group('getUserRole', () {
    String fakeJwt(Map<String, dynamic> claims) {
      String seg(Map<String, dynamic> m) => base64Url
          .encode(utf8.encode(json.encode(m)))
          .replaceAll('=', '');
      return '${seg({'alg': 'none'})}.${seg(claims)}.sig';
    }

    test('reads the role from the access token without calling userinfo', () async {
      final token = fakeJwt({
        'sub': 'user-1',
        'role': 'worker',
        'exp': 9999999999,
      });

      final result = await datasource.getUserRole(token);

      expect(result, 'worker');
      verifyNever(() => mockDio.get(any(), options: any(named: 'options')));
      verifyNever(() => mockNetwork.isConnected);
    });

    test('prefers worker when the token carries multiple roles', () async {
      final token = fakeJwt({
        'role': ['admin', 'worker'],
        'exp': 9999999999,
      });

      expect(await datasource.getUserRole(token), 'worker');
    });

    test('falls back to userinfo when the token has no role claim', () async {
      final token = fakeJwt({'sub': 'user-1', 'exp': 9999999999});
      when(() => mockNetwork.isConnected).thenAnswer((_) async => true);
      when(() => mockDio.get(any(), options: any(named: 'options'))).thenAnswer(
        (_) async => Response(
          requestOptions: tRequestOptions,
          statusCode: 200,
          data: <String, dynamic>{'role': 'worker'},
        ),
      );

      final result = await datasource.getUserRole(token);

      expect(result, 'worker');
      final url = verify(
        () => mockDio.get(captureAny(), options: any(named: 'options')),
      ).captured.single as String;
      expect(url, endsWith('/connect/userinfo'));
      expect(url, isNot(contains('//connect')));
    });
  });

  group('revokeRefreshToken', () {
    const tRefreshToken = 'refresh-456';

    test('posts the refresh token form-urlencoded to /connect/revocation',
        () async {
      when(() => mockNetwork.isConnected).thenAnswer((_) async => true);
      when(() => mockAnonymousDio.post(
            any(),
            data: any(named: 'data'),
            options: any(named: 'options'),
          )).thenAnswer(
        (_) async => Response(
          requestOptions: tRequestOptions,
          statusCode: 200,
        ),
      );

      await datasource.revokeRefreshToken(tRefreshToken);

      final captured = verify(() => mockAnonymousDio.post(
            captureAny(),
            data: captureAny(named: 'data'),
            options: captureAny(named: 'options'),
          )).captured;
      expect(captured[0] as String, endsWith('/connect/revocation'));
      final body = captured[1] as Map;
      expect(body['token'], tRefreshToken);
      expect(body['token_type_hint'], 'refresh_token');
      expect(body.containsKey('client_id'), true);
      expect(
        (captured[2] as Options).contentType,
        Headers.formUrlEncodedContentType,
      );
    });

    test('throws ServerException on server error', () async {
      when(() => mockNetwork.isConnected).thenAnswer((_) async => true);
      when(() => mockAnonymousDio.post(
            any(),
            data: any(named: 'data'),
            options: any(named: 'options'),
          )).thenThrow(badResponse(500));

      await expectLater(
        () => datasource.revokeRefreshToken(tRefreshToken),
        throwsA(isA<ServerException>()),
      );
    });

    test('throws NetworkException without calling the endpoint when offline',
        () async {
      when(() => mockNetwork.isConnected).thenAnswer((_) async => false);

      await expectLater(
        () => datasource.revokeRefreshToken(tRefreshToken),
        throwsA(isA<NetworkException>()),
      );
      verifyNever(() => mockAnonymousDio.post(
            any(),
            data: any(named: 'data'),
            options: any(named: 'options'),
          ));
    });
  });

  group('requestPasswordResetCode', () {
    test('posts the email to /Password/forgot', () async {
      when(() => mockNetwork.isConnected).thenAnswer((_) async => true);
      when(() => mockAnonymousDio.post(any(), data: any(named: 'data')))
          .thenAnswer(
        (_) async => Response(
          requestOptions: tRequestOptions,
          statusCode: 202,
        ),
      );

      await datasource.requestPasswordResetCode(tEmail);

      final captured = verify(() => mockAnonymousDio.post(
            captureAny(),
            data: captureAny(named: 'data'),
          )).captured;
      expect(captured[0] as String, endsWith('/Password/forgot'));
      expect(captured[1], {'email': tEmail});
    });

    test('throws NetworkException when offline', () async {
      when(() => mockNetwork.isConnected).thenAnswer((_) async => false);

      await expectLater(
        () => datasource.requestPasswordResetCode(tEmail),
        throwsA(isA<NetworkException>()),
      );
      verifyNever(() => mockAnonymousDio.post(any(), data: any(named: 'data')));
    });
  });

  group('resetPassword', () {
    test('posts email, code and newPassword to /Password/reset', () async {
      when(() => mockNetwork.isConnected).thenAnswer((_) async => true);
      when(() => mockAnonymousDio.post(any(), data: any(named: 'data')))
          .thenAnswer(
        (_) async => Response(
          requestOptions: tRequestOptions,
          statusCode: 200,
        ),
      );

      await datasource.resetPassword(
        email: tEmail,
        code: '123456',
        newPassword: 'newPass1',
      );

      final captured = verify(() => mockAnonymousDio.post(
            captureAny(),
            data: captureAny(named: 'data'),
          )).captured;
      expect(captured[0] as String, endsWith('/Password/reset'));
      expect(captured[1], {
        'email': tEmail,
        'code': '123456',
        'newPassword': 'newPass1',
      });
    });

    for (final entry in {
      'invalid_code': ErrorMessages.invalidResetCode,
      'code_expired': ErrorMessages.resetCodeExpired,
      'too_many_attempts': ErrorMessages.tooManyResetAttempts,
    }.entries) {
      test('maps reset error ${entry.key} on 400', () async {
        when(() => mockNetwork.isConnected).thenAnswer((_) async => true);
        when(() => mockAnonymousDio.post(any(), data: any(named: 'data')))
            .thenThrow(
          badResponse(400, {'error': entry.key, 'messages': <String>[]}),
        );

        await expectLater(
          () => datasource.resetPassword(
            email: tEmail,
            code: '123456',
            newPassword: 'newPass1',
          ),
          throwsA(
            isA<ServerException>()
                .having((e) => e.message, 'message', entry.value)
                .having((e) => e.code, 'code', entry.key),
          ),
        );
      });
    }

    test('appends policy messages on password_policy error', () async {
      when(() => mockNetwork.isConnected).thenAnswer((_) async => true);
      when(() => mockAnonymousDio.post(any(), data: any(named: 'data')))
          .thenThrow(
        badResponse(400, {
          'error': 'password_policy',
          'messages': ['Passwords must be at least 6 characters.'],
        }),
      );

      await expectLater(
        () => datasource.resetPassword(
          email: tEmail,
          code: '123456',
          newPassword: 'short',
        ),
        throwsA(
          isA<ServerException>()
              .having(
                (e) => e.message,
                'message',
                contains('Passwords must be at least 6 characters.'),
              )
              .having((e) => e.code, 'code', 'password_policy'),
        ),
      );
    });
  });

  group('resendConfirmationLink', () {
    test('posts userName as a query parameter', () async {
      when(() => mockNetwork.isConnected).thenAnswer((_) async => true);
      when(() => mockAnonymousDio.post(
            any(),
            queryParameters: any(named: 'queryParameters'),
          )).thenAnswer(
        (_) async => Response(
          requestOptions: tRequestOptions,
          statusCode: 200,
        ),
      );

      await datasource.resendConfirmationLink(tEmail);

      final captured = verify(() => mockAnonymousDio.post(
            captureAny(),
            queryParameters: captureAny(named: 'queryParameters'),
          )).captured;
      expect(
        captured[0] as String,
        endsWith('/Account/ResendConfirmationLink'),
      );
      expect(captured[1], {'userName': tEmail});
    });

    test('throws NetworkException when offline', () async {
      when(() => mockNetwork.isConnected).thenAnswer((_) async => false);

      await expectLater(
        () => datasource.resendConfirmationLink(tEmail),
        throwsA(isA<NetworkException>()),
      );
    });
  });
}
