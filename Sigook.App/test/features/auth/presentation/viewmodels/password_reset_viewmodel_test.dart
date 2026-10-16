import 'dart:async';

import 'package:dartz/dartz.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:mocktail/mocktail.dart';
import 'package:sigook_app_flutter/core/constants/error_messages.dart';
import 'package:sigook_app_flutter/core/error/failures.dart';
import 'package:sigook_app_flutter/features/auth/domain/usecases/request_password_reset_code.dart';
import 'package:sigook_app_flutter/features/auth/domain/usecases/reset_password.dart';
import 'package:sigook_app_flutter/features/auth/presentation/providers/auth_providers.dart';
import 'package:sigook_app_flutter/features/auth/presentation/viewmodels/password_reset_viewmodel.dart';

import '../../../../helpers/riverpod_test_helpers.dart';

class MockRequestPasswordResetCode extends Mock
    implements RequestPasswordResetCode {}

class MockResetPassword extends Mock implements ResetPassword {}

void main() {
  setUpAll(() {
    registerFallbackValue(RequestPasswordResetCodeParams(email: ''));
    registerFallbackValue(
      ResetPasswordParams(email: '', code: '', newPassword: ''),
    );
  });

  late MockRequestPasswordResetCode mockRequestCode;
  late MockResetPassword mockResetPassword;

  const tEmail = 'test@example.com';

  setUp(() {
    mockRequestCode = MockRequestPasswordResetCode();
    mockResetPassword = MockResetPassword();
  });

  ProviderContainer buildTestContainer() {
    return buildContainer(ProviderContainer(overrides: [
      requestPasswordResetCodeProvider.overrideWithValue(mockRequestCode),
      resetPasswordProvider.overrideWithValue(mockResetPassword),
    ]));
  }

  test('requestCode success advances to step 2 and starts the cooldown',
      () async {
    final container = buildTestContainer();
    var codeSentPulsed = false;
    container.listen(passwordResetViewModelProvider, (previous, next) {
      if (previous?.justCodeSent != true && next.justCodeSent) {
        codeSentPulsed = true;
      }
    });
    when(() => mockRequestCode.call(any()))
        .thenAnswer((_) async => const Right(null));

    await container
        .read(passwordResetViewModelProvider.notifier)
        .requestCode(tEmail);

    final state = container.read(passwordResetViewModelProvider);
    expect(state.step, 2);
    expect(state.email, tEmail);
    expect(state.isLoading, false);
    expect(state.resendCooldownSeconds, greaterThan(0));
    expect(state.justCodeSent, false);
    expect(codeSentPulsed, true);
  });

  test('requestCode failure stays on step 1 with a send error', () async {
    final container = buildTestContainer();
    container.listen(passwordResetViewModelProvider, (_, _) {});
    when(() => mockRequestCode.call(any())).thenAnswer(
      (_) async => const Left(ServerFailure(message: 'boom')),
    );

    await container
        .read(passwordResetViewModelProvider.notifier)
        .requestCode(tEmail);

    final state = container.read(passwordResetViewModelProvider);
    expect(state.step, 1);
    expect(state.error, ErrorMessages.resetCodeSendFailed);
    expect(state.isLoading, false);
  });

  test('requestCode is a no-op while another request is in flight', () async {
    final container = buildTestContainer();
    container.listen(passwordResetViewModelProvider, (_, _) {});
    final completer = Completer<Either<Failure, void>>();
    when(() => mockRequestCode.call(any())).thenAnswer((_) => completer.future);

    final notifier = container.read(passwordResetViewModelProvider.notifier);
    final pending = notifier.requestCode(tEmail);
    await notifier.requestCode(tEmail);

    verify(() => mockRequestCode.call(any())).called(1);
    completer.complete(const Right(null));
    await pending;
  });

  test('resetPassword is a no-op while another request is in flight', () async {
    final container = buildTestContainer();
    container.listen(passwordResetViewModelProvider, (_, _) {});
    final completer = Completer<Either<Failure, void>>();
    when(() => mockRequestCode.call(any())).thenAnswer((_) => completer.future);

    final notifier = container.read(passwordResetViewModelProvider.notifier);
    final pending = notifier.requestCode(tEmail);
    await notifier.resetPassword(code: '123456', newPassword: 'newPass1');

    verifyNever(() => mockResetPassword.call(any()));
    completer.complete(const Right(null));
    await pending;
  });

  test('resendCode is a no-op while the cooldown is active', () async {
    final container = buildTestContainer();
    container.listen(passwordResetViewModelProvider, (_, _) {});
    when(() => mockRequestCode.call(any()))
        .thenAnswer((_) async => const Right(null));

    final notifier = container.read(passwordResetViewModelProvider.notifier);
    await notifier.requestCode(tEmail);
    await notifier.resendCode();

    verify(() => mockRequestCode.call(any())).called(1);
  });

  test('resetPassword success pulses justReset using the stored email',
      () async {
    final container = buildTestContainer();
    var resetPulsed = false;
    container.listen(passwordResetViewModelProvider, (previous, next) {
      if (previous?.justReset != true && next.justReset) {
        resetPulsed = true;
      }
    });
    when(() => mockRequestCode.call(any()))
        .thenAnswer((_) async => const Right(null));
    when(() => mockResetPassword.call(any()))
        .thenAnswer((_) async => const Right(null));

    final notifier = container.read(passwordResetViewModelProvider.notifier);
    await notifier.requestCode(tEmail);
    await notifier.resetPassword(code: '123456', newPassword: 'newPass1');

    expect(resetPulsed, true);
    expect(container.read(passwordResetViewModelProvider).isLoading, false);
    final captured = verify(() => mockResetPassword.call(captureAny()))
        .captured
        .single as ResetPasswordParams;
    expect(captured.email, tEmail);
    expect(captured.code, '123456');
    expect(captured.newPassword, 'newPass1');
  });

  test('continueToPassword advances from step 2 to step 3', () async {
    final container = buildTestContainer();
    container.listen(passwordResetViewModelProvider, (_, _) {});
    when(() => mockRequestCode.call(any()))
        .thenAnswer((_) async => const Right(null));

    final notifier = container.read(passwordResetViewModelProvider.notifier);
    await notifier.requestCode(tEmail);
    notifier.continueToPassword();

    expect(container.read(passwordResetViewModelProvider).step, 3);
  });

  test('continueToPassword is a no-op outside step 2', () async {
    final container = buildTestContainer();
    container.listen(passwordResetViewModelProvider, (_, _) {});

    container
        .read(passwordResetViewModelProvider.notifier)
        .continueToPassword();

    expect(container.read(passwordResetViewModelProvider).step, 1);
  });

  test('backToCode returns from step 3 to step 2', () async {
    final container = buildTestContainer();
    container.listen(passwordResetViewModelProvider, (_, _) {});
    when(() => mockRequestCode.call(any()))
        .thenAnswer((_) async => const Right(null));

    final notifier = container.read(passwordResetViewModelProvider.notifier);
    await notifier.requestCode(tEmail);
    notifier.continueToPassword();
    notifier.backToCode();

    final state = container.read(passwordResetViewModelProvider);
    expect(state.step, 2);
    expect(state.error, isNull);
    expect(state.errorCode, isNull);
  });

  test('resetPassword code failure returns to the code step', () async {
    final container = buildTestContainer();
    container.listen(passwordResetViewModelProvider, (_, _) {});
    when(() => mockRequestCode.call(any()))
        .thenAnswer((_) async => const Right(null));
    when(() => mockResetPassword.call(any())).thenAnswer(
      (_) async => const Left(
        ServerFailure(
          message: 'Invalid code',
          statusCode: 400,
          code: 'invalid_code',
        ),
      ),
    );

    final notifier = container.read(passwordResetViewModelProvider.notifier);
    await notifier.requestCode(tEmail);
    notifier.continueToPassword();
    await notifier.resetPassword(code: '000000', newPassword: 'NewPass1');

    final state = container.read(passwordResetViewModelProvider);
    expect(state.step, 2);
    expect(state.error, 'Invalid code');
    expect(state.errorCode, 'invalid_code');
  });

  test('resetPassword password_policy failure stays on the password step',
      () async {
    final container = buildTestContainer();
    container.listen(passwordResetViewModelProvider, (_, _) {});
    when(() => mockRequestCode.call(any()))
        .thenAnswer((_) async => const Right(null));
    when(() => mockResetPassword.call(any())).thenAnswer(
      (_) async => const Left(
        ServerFailure(
          message: 'The new password was rejected.',
          statusCode: 400,
          code: 'password_policy',
        ),
      ),
    );

    final notifier = container.read(passwordResetViewModelProvider.notifier);
    await notifier.requestCode(tEmail);
    notifier.continueToPassword();
    await notifier.resetPassword(code: '123456', newPassword: 'weakpass');

    final state = container.read(passwordResetViewModelProvider);
    expect(state.step, 3);
    expect(state.errorCode, 'password_policy');
  });

  test('resetPassword failure sets error and errorCode', () async {
    final container = buildTestContainer();
    container.listen(passwordResetViewModelProvider, (_, _) {});
    when(() => mockResetPassword.call(any())).thenAnswer(
      (_) async => const Left(
        ServerFailure(
          message: 'Invalid code',
          statusCode: 400,
          code: 'invalid_code',
        ),
      ),
    );

    await container
        .read(passwordResetViewModelProvider.notifier)
        .resetPassword(code: '000000', newPassword: 'newPass1');

    final state = container.read(passwordResetViewModelProvider);
    expect(state.error, 'Invalid code');
    expect(state.errorCode, 'invalid_code');
    expect(state.isLoading, false);
  });
}
