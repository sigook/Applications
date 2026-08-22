import 'package:dartz/dartz.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:mocktail/mocktail.dart';
import 'package:sigook_app_flutter/core/error/failures.dart';
import 'package:sigook_app_flutter/features/auth/domain/usecases/resend_confirmation_link.dart';

import '../../../../helpers/mocks.dart';

void main() {
  late MockAuthRepository mockRepo;
  late ResendConfirmationLink usecase;

  const tEmail = 'test@example.com';

  setUp(() {
    mockRepo = MockAuthRepository();
    usecase = ResendConfirmationLink(mockRepo);
  });

  test('delegates the email to the repository and returns success', () async {
    when(() => mockRepo.resendConfirmationLink(tEmail))
        .thenAnswer((_) async => const Right(null));

    final result = await usecase(ResendConfirmationLinkParams(email: tEmail));

    expect(result.isRight(), true);
    verify(() => mockRepo.resendConfirmationLink(tEmail)).called(1);
  });

  test('returns the failure from the repository', () async {
    const failure = ServerFailure(message: 'send failed');
    when(() => mockRepo.resendConfirmationLink(tEmail))
        .thenAnswer((_) async => const Left(failure));

    final result = await usecase(ResendConfirmationLinkParams(email: tEmail));

    expect(result, const Left(failure));
  });
}
