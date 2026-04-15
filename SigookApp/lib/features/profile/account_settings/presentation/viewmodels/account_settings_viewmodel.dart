import 'package:freezed_annotation/freezed_annotation.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';
import '../../../presentation/providers/cached_worker_profile_provider.dart';
import '../providers/account_settings_providers.dart';

part 'account_settings_viewmodel.freezed.dart';
part 'account_settings_viewmodel.g.dart';

@freezed
abstract class AccountSettingsState with _$AccountSettingsState {
  const factory AccountSettingsState({
    @Default(false) bool isChangingEmail,
    @Default(false) bool isSavingEmail,
    String? emailError,
    @Default(false) bool justChangedEmail,
  }) = _AccountSettingsState;
}

@riverpod
class AccountSettingsViewModel extends _$AccountSettingsViewModel {
  @override
  AccountSettingsState build() => const AccountSettingsState();

  void startChangingEmail() => state = state.copyWith(
        isChangingEmail: true,
        emailError: null,
        justChangedEmail: false,
      );

  void cancelChangingEmail() => state = state.copyWith(isChangingEmail: false);

  Future<void> changeEmail({
    required String newEmail,
    required String confirmNewEmail,
  }) async {
    state = state.copyWith(isSavingEmail: true, emailError: null, justChangedEmail: false);

    final result = await ref.read(changeEmailUseCaseProvider)(
      newEmail: newEmail,
      confirmNewEmail: confirmNewEmail,
    );

    result.fold(
      (failure) => state = state.copyWith(
        isSavingEmail: false,
        emailError: failure.message,
      ),
      (_) {
        state = state.copyWith(
          isSavingEmail: false,
          isChangingEmail: false,
          justChangedEmail: true,
        );
        ref.invalidate(cachedWorkerProfileProvider);
      },
    );
  }
}
