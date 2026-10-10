import 'dart:async';
import 'package:freezed_annotation/freezed_annotation.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';
import '../../../../core/constants/auth_error_codes.dart';
import '../../../../core/constants/error_messages.dart';
import '../../../../core/error/failures.dart';
import '../../domain/usecases/request_password_reset_code.dart';
import '../../domain/usecases/reset_password.dart';
import '../providers/auth_providers.dart';

part 'password_reset_viewmodel.freezed.dart';
part 'password_reset_viewmodel.g.dart';

@freezed
sealed class PasswordResetState with _$PasswordResetState {
  const factory PasswordResetState({
    @Default(1) int step,
    @Default('') String email,
    @Default(false) bool isLoading,
    String? error,
    String? errorCode,
    @Default(0) int resendCooldownSeconds,
    @Default(false) bool justCodeSent,
    @Default(false) bool justReset,
  }) = _PasswordResetState;
}

@riverpod
class PasswordResetViewModel extends _$PasswordResetViewModel {
  Timer? _cooldownTimer;

  @override
  PasswordResetState build() {
    ref.onDispose(() => _cooldownTimer?.cancel());
    return const PasswordResetState();
  }

  Future<void> requestCode(String email) async {
    if (state.isLoading) return;
    state = state.copyWith(isLoading: true, error: null, errorCode: null);

    final useCase = ref.read(requestPasswordResetCodeProvider);
    final result = await useCase(RequestPasswordResetCodeParams(email: email));

    if (!ref.mounted) return;

    result.fold(
      (failure) {
        state = state.copyWith(
          isLoading: false,
          error: ErrorMessages.resetCodeSendFailed,
        );
      },
      (_) {
        state = state.copyWith(
          isLoading: false,
          step: 2,
          email: email,
          justCodeSent: true,
        );
        state = state.copyWith(justCodeSent: false);
        _startCooldown();
      },
    );
  }

  Future<void> resendCode() async {
    if (state.resendCooldownSeconds > 0 || state.email.isEmpty) return;
    await requestCode(state.email);
  }

  void continueToPassword() {
    if (state.step != 2) return;
    state = state.copyWith(step: 3, error: null, errorCode: null);
  }

  void backToCode() {
    if (state.step != 3) return;
    state = state.copyWith(step: 2, error: null, errorCode: null);
  }

  Future<void> resetPassword({
    required String code,
    required String newPassword,
  }) async {
    if (state.isLoading) return;
    state = state.copyWith(isLoading: true, error: null, errorCode: null);

    final useCase = ref.read(resetPasswordProvider);
    final result = await useCase(
      ResetPasswordParams(
        email: state.email,
        code: code,
        newPassword: newPassword,
      ),
    );

    if (!ref.mounted) return;

    result.fold(
      (failure) {
        final code = failure is ServerFailure ? failure.code : null;
        final isCodeIssue =
            code == AuthErrorCodes.invalidCode ||
            code == AuthErrorCodes.codeExpired ||
            code == AuthErrorCodes.tooManyAttempts;
        state = state.copyWith(
          isLoading: false,
          error: failure.message,
          errorCode: code,
          step: isCodeIssue && state.step == 3 ? 2 : state.step,
        );
      },
      (_) {
        state = state.copyWith(isLoading: false, justReset: true);
        state = state.copyWith(justReset: false);
      },
    );
  }

  void _startCooldown() {
    _cooldownTimer?.cancel();
    state = state.copyWith(resendCooldownSeconds: 60);
    _cooldownTimer = Timer.periodic(const Duration(seconds: 1), (timer) {
      if (!ref.mounted) {
        timer.cancel();
        return;
      }
      final remaining = state.resendCooldownSeconds - 1;
      state = state.copyWith(resendCooldownSeconds: remaining < 0 ? 0 : remaining);
      if (remaining <= 0) {
        timer.cancel();
      }
    });
  }
}
