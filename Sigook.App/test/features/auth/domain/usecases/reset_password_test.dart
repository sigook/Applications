import 'package:dartz/dartz.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:mocktail/mocktail.dart';
import 'package:sigook_app_flutter/core/error/failures.dart';
import 'package:sigook_app_flutter/features/auth/domain/usecases/reset_password.dart';

import '../../../../helpers/mocks.dart';

void main() {
  late MockAuthRepository mockRepo;
  late ResetPassword usecase;

  const tEmail = 'test@example.com';
  const tCode = '123456';
  const tNewPassword = 'newPass1';

  setUp(() {
    mockRepo = MockAuthRepository();
    usecase = ResetPassword(mockRepo);
  });

  test('delegates all params to the repository and returns success', () async {
    when(() => mockRepo.resetPassword(
          email: tEmail,
          code: tCode,
          newPassword: tNewPassword,
        )).thenAnswer((_) async => const Right(null));

    final result = await usecase(
      ResetPasswordParams(
        email: tEmail,
        code: tCode,
        newPassword: tNewPassword,
      ),
    );

    expect(result.isRight(), true);
    verify(() => mockRepo.resetPassword(
          email: tEmail,
          code: tCode,
          newPassword: tNewPassword,
        )).called(1);
  });

  test('returns the failure from the repository', () async {
    const failure = ServerFailure(message: 'Invalid code', code: 'invalid_code');
    when(() => mockRepo.resetPassword(
          email: any(named: 'email'),
          code: any(named: 'code'),
          newPassword: any(named: 'newPassword'),
        )).thenAnswer((_) async => const Left(failure));

    final result = await usecase(
      ResetPasswordParams(
        email: tEmail,
        code: tCode,
        newPassword: tNewPassword,
      ),
    );

    expect(result, const Left(failure));
  });
}
