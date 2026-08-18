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

  final tRequestOptions = RequestOptions(path: '/Account/Login');

  DioException badResponse(int statusCode) => DioException(
        requestOptions: tRequestOptions,
        response: Response(
          requestOptions: tRequestOptions,
          statusCode: statusCode,
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
    test('posts credentials to /Account/Login and maps the response',
        () async {
      when(() => mockNetwork.isConnected).thenAnswer((_) async => true);
      when(() => mockAnonymousDio.post(any(), data: any(named: 'data')))
          .thenAnswer(
        (_) async => Response(
          requestOptions: tRequestOptions,
          statusCode: 200,
          data: {
            'accessToken': 'access-123',
            'refreshToken': 'refresh-456',
            'tokenType': 'Bearer',
            'expiresIn': 3600,
          },
        ),
      );

      final result = await datasource.signIn(
        email: tEmail,
        password: tPassword,
      );

      expect(result.accessToken, 'access-123');
      expect(result.refreshToken, 'refresh-456');
      expect(result.tokenType, 'Bearer');
      expect(result.expirationDateTime, isNotNull);
      verify(() => mockAnonymousDio.post(
            '/Account/Login',
            data: {'email': tEmail, 'password': tPassword},
          )).called(1);
    });

    test('throws ServerException with invalid credentials message on 401',
        () async {
      when(() => mockNetwork.isConnected).thenAnswer((_) async => true);
      when(() => mockAnonymousDio.post(any(), data: any(named: 'data')))
          .thenThrow(badResponse(401));

      await expectLater(
        () => datasource.signIn(email: tEmail, password: tPassword),
        throwsA(
          isA<ServerException>()
              .having((e) => e.message, 'message',
                  ErrorMessages.invalidCredentials)
              .having((e) => e.statusCode, 'statusCode', 401),
        ),
      );
    });

    test('throws ServerException with invalid credentials message on 400',
        () async {
      when(() => mockNetwork.isConnected).thenAnswer((_) async => true);
      when(() => mockAnonymousDio.post(any(), data: any(named: 'data')))
          .thenThrow(badResponse(400));

      await expectLater(
        () => datasource.signIn(email: tEmail, password: tPassword),
        throwsA(
          isA<ServerException>()
              .having((e) => e.message, 'message',
                  ErrorMessages.invalidCredentials)
              .having((e) => e.statusCode, 'statusCode', 400),
        ),
      );
    });

    test('throws generic ServerException on 404 (endpoint not shipped yet)',
        () async {
      when(() => mockNetwork.isConnected).thenAnswer((_) async => true);
      when(() => mockAnonymousDio.post(any(), data: any(named: 'data')))
          .thenThrow(badResponse(404));

      await expectLater(
        () => datasource.signIn(email: tEmail, password: tPassword),
        throwsA(
          isA<ServerException>()
              .having((e) => e.message, 'message', 'Server error: 404')
              .having((e) => e.statusCode, 'statusCode', 404),
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
      verifyNever(() => mockAnonymousDio.post(any(), data: any(named: 'data')));
    });

    test('throws NetworkException on connection timeout', () async {
      when(() => mockNetwork.isConnected).thenAnswer((_) async => true);
      when(() => mockAnonymousDio.post(any(), data: any(named: 'data')))
          .thenThrow(
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
}
