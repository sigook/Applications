import 'package:riverpod_annotation/riverpod_annotation.dart';
import '../../domain/entities/basic_info.dart';
import '../../domain/entities/preferences_info.dart';
import '../../domain/entities/documents_info.dart';
import '../../domain/entities/account_info.dart';
import '../../domain/entities/registration_form.dart';
import '../../domain/repositories/registration_repository.dart';
import '../../domain/usecases/submit_registration.dart';
import '../providers/registration_providers.dart';

part 'registration_viewmodel.g.dart';

/// ViewModel for managing registration form state
@riverpod
class RegistrationViewModel extends _$RegistrationViewModel {
  @override
  RegistrationForm build() {
    _loadDraft();
    return RegistrationForm.empty();
  }

  RegistrationRepository get _repository => ref.read(registrationRepositoryProvider);
  SubmitRegistration get _submitUseCase => ref.read(submitRegistrationUseCaseProvider);

  /// Load saved draft
  Future<void> _loadDraft() async {
    final result = await _repository.getDraft();
    result.fold(
      (failure) => {}, // Ignore error, start with empty form
      (form) => state = form,
    );
  }

  /// Update basic info section (Section 1)
  void updateBasicInfo(BasicInfo info) {
    state = state.copyWith(basicInfo: info);
    _saveDraft();
  }

  /// Update preferences info section (Section 2)
  void updatePreferencesInfo(PreferencesInfo info) {
    state = state.copyWith(preferencesInfo: info);
    _saveDraft();
  }

  /// Update documents info section (Section 3)
  void updateDocumentsInfo(DocumentsInfo info) {
    state = state.copyWith(documentsInfo: info);
    _saveDraft();
  }

  /// Update account info section (Section 4)
  void updateAccountInfo(AccountInfo info) {
    state = state.copyWith(accountInfo: info);
    _saveDraft();
  }

  /// Save draft
  Future<void> _saveDraft() async {
    await _repository.saveDraft(state);
  }

  /// Submit registration
  Future<bool> submitRegistration() async {
    final result = await _submitUseCase(state);
    return result.fold(
      (failure) => false,
      (_) => true,
    );
  }

  /// Check email availability
  Future<bool> checkEmailAvailability(String email) async {
    final result = await _repository.isEmailAvailable(email);
    return result.fold(
      (failure) => false,
      (isAvailable) => isAvailable,
    );
  }

  /// Clear all form data
  Future<void> clearForm() async {
    await _repository.clearDraft();
    state = RegistrationForm.empty();
  }
}
