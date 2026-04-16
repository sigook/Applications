import 'package:dartz/dartz.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:mocktail/mocktail.dart';
import 'package:sigook_app_flutter/core/error/failures.dart';
import 'package:sigook_app_flutter/features/auth/domain/entities/auth_token.dart';
import 'package:sigook_app_flutter/features/auth/domain/usecases/refresh_token.dart';

import '../../../../helpers/mocks.dart';

void main() {
  late MockAuthRepository mockRepo;
  late RefreshToken usecase;

  const tRefreshToken = 'refresh-token-xyz';
  const tToken = AuthToken(accessToken: 'new-access', refreshToken: 'new-refresh');

  setUp(() {
    mockRepo = MockAuthRepository();
    usecase = RefreshToken(mockRepo);
  });

  test('returns new AuthToken when repository succeeds', () async {
    when(() => mockRepo.refreshToken(tRefreshToken))
        .thenAnswer((_) async => const Right(tToken));

    final result = await usecase(
      RefreshTokenParams(refreshToken: tRefreshToken),
    );

    expect(result, const Right(tToken));
    verify(() => mockRepo.refreshToken(tRefreshToken)).called(1);
  });

  test('passes the refresh token string to the repository', () async {
    when(() => mockRepo.refreshToken(any()))
        .thenAnswer((_) async => const Right(tToken));

    await usecase(RefreshTokenParams(refreshToken: 'specific-token'));

    verify(() => mockRepo.refreshToken('specific-token')).called(1);
  });

  test('returns NetworkFailure when repository fails', () async {
    const failure = NetworkFailure(message: 'no connection');
    when(() => mockRepo.refreshToken(any()))
        .thenAnswer((_) async => const Left(failure));

    final result = await usecase(
      RefreshTokenParams(refreshToken: tRefreshToken),
    );

    expect(result, const Left(failure));
  });
}
