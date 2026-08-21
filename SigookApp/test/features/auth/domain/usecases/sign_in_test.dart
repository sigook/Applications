import 'package:dartz/dartz.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:mocktail/mocktail.dart';
import 'package:sigook_app_flutter/core/error/failures.dart';
import 'package:sigook_app_flutter/features/auth/domain/entities/auth_token.dart';
import 'package:sigook_app_flutter/features/auth/domain/usecases/sign_in.dart';

import '../../../../helpers/mocks.dart';

void main() {
  late MockAuthRepository mockRepo;
  late SignIn usecase;

  const tToken = AuthToken(accessToken: 'access', refreshToken: 'refresh');
  const tEmail = 'test@example.com';
  const tPassword = 'password123';

  setUp(() {
    mockRepo = MockAuthRepository();
    usecase = SignIn(mockRepo);
  });

  test('returns AuthToken when repository succeeds', () async {
    when(() => mockRepo.signIn(
          email: any(named: 'email'),
          password: any(named: 'password'),
        )).thenAnswer((_) async => const Right(tToken));

    final result = await usecase(
      SignInParams(email: tEmail, password: tPassword),
    );

    expect(result, const Right(tToken));
    verify(() => mockRepo.signIn(email: tEmail, password: tPassword))
        .called(1);
  });

  test('returns ServerFailure when repository fails', () async {
    const failure = ServerFailure(message: 'Invalid credentials');
    when(() => mockRepo.signIn(
          email: any(named: 'email'),
          password: any(named: 'password'),
        )).thenAnswer((_) async => const Left(failure));

    final result = await usecase(
      SignInParams(email: tEmail, password: tPassword),
    );

    expect(result, const Left(failure));
    verify(() => mockRepo.signIn(email: tEmail, password: tPassword))
        .called(1);
  });
}
