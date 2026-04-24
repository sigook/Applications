import 'package:dartz/dartz.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:mocktail/mocktail.dart';
import 'package:sigook_app_flutter/core/error/failures.dart';
import 'package:sigook_app_flutter/features/auth/domain/usecases/validate_token.dart';

import '../../../../helpers/mocks.dart';

void main() {
  late MockAuthRepository mockRepo;
  late ValidateToken usecase;

  const tAccessToken = 'access-token-abc';

  setUp(() {
    mockRepo = MockAuthRepository();
    usecase = ValidateToken(mockRepo);
  });

  test('returns true when token is valid', () async {
    when(() => mockRepo.validateToken(tAccessToken))
        .thenAnswer((_) async => const Right(true));

    final result = await usecase(
      ValidateTokenParams(accessToken: tAccessToken),
    );

    expect(result, const Right(true));
    verify(() => mockRepo.validateToken(tAccessToken)).called(1);
  });

  test('returns false when token is invalid', () async {
    when(() => mockRepo.validateToken(tAccessToken))
        .thenAnswer((_) async => const Right(false));

    final result = await usecase(
      ValidateTokenParams(accessToken: tAccessToken),
    );

    expect(result, const Right(false));
  });

  test('returns ServerFailure when repository fails', () async {
    const failure = ServerFailure(message: 'validation endpoint error');
    when(() => mockRepo.validateToken(any()))
        .thenAnswer((_) async => const Left(failure));

    final result = await usecase(
      ValidateTokenParams(accessToken: tAccessToken),
    );

    expect(result, const Left(failure));
  });

  test('passes access token to repository', () async {
    when(() => mockRepo.validateToken(any()))
        .thenAnswer((_) async => const Right(true));

    await usecase(ValidateTokenParams(accessToken: 'my-token'));

    verify(() => mockRepo.validateToken('my-token')).called(1);
  });
}
