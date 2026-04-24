import 'package:flutter/material.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';
import '../../../../core/providers/analytics_providers.dart';
import '../../domain/entities/basic_info.dart';
import '../../domain/entities/preferences_info.dart';
import '../../domain/entities/documents_info.dart';
import '../../domain/entities/account_info.dart';
import '../../domain/entities/registration_form.dart';
import '../../domain/repositories/registration_repository.dart';
import '../../domain/usecases/submit_registration.dart';
import '../providers/registration_providers.dart';

part 'registration_viewmodel.g.dart';

@riverpod
class RegistrationViewModel extends _$RegistrationViewModel {
  @override
  RegistrationForm build() {
    _loadDraft();
    return RegistrationForm.empty();
  }

  RegistrationRepository get _repository =>
      ref.read(registrationRepositoryProvider);
  SubmitRegistration get _submitUseCase =>
      ref.read(submitRegistrationUseCaseProvider);

  Future<void> _loadDraft() async {
    final result = await _repository.getDraft();
    result.fold((failure) => {}, (form) => state = form);
  }

  void updateBasicInfo(BasicInfo info) {
    debugPrint('updateBasicInfo: new photo path = ${info.profilePhoto.path}');
    state = state.copyWith(basicInfo: info);
    debugPrint('State updated');
    _saveDraft();
    ref.read(analyticsServiceProvider).logEvent(
      name: 'registration_step_completed',
      parameters: {'step': '0', 'step_name': 'basic_info'},
    );
  }

  void updatePreferencesInfo(PreferencesInfo info) {
    state = state.copyWith(preferencesInfo: info);
    _saveDraft();
    ref.read(analyticsServiceProvider).logEvent(
      name: 'registration_step_completed',
      parameters: {'step': '1', 'step_name': 'preferences'},
    );
  }

  void updateDocumentsInfo(DocumentsInfo info) {
    state = state.copyWith(documentsInfo: info);
    _saveDraft();
    ref.read(analyticsServiceProvider).logEvent(
      name: 'registration_step_completed',
      parameters: {'step': '2', 'step_name': 'documents'},
    );
  }

  void updateAccountInfo(AccountInfo info) {
    state = state.copyWith(accountInfo: info);
    _saveDraft();
    ref.read(analyticsServiceProvider).logEvent(
      name: 'registration_step_completed',
      parameters: {'step': '3', 'step_name': 'account'},
    );
  }

  Future<void> _saveDraft() async {
    await _repository.saveDraft(state);
  }

  Future<String> submitRegistration() async {
    final result = await _submitUseCase(state);
    return result.fold(
      (failure) {
        ref.read(analyticsServiceProvider).logEvent(
          name: 'registration_failed',
          parameters: {'error': failure.message},
        );
        return failure.message;
      },
      (_) {
        ref.read(analyticsServiceProvider).logSignUp(method: 'email');
        ref.read(analyticsServiceProvider).logEvent(
          name: 'registration_completed',
        );
        return 'Success';
      },
    );
  }

  Future<void> clearForm() async {
    await _repository.clearDraft();
    state = RegistrationForm.empty();
  }
}
