import 'package:flutter/material.dart';
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
  }

  void updatePreferencesInfo(PreferencesInfo info) {
    state = state.copyWith(preferencesInfo: info);
    _saveDraft();
  }

  void updateDocumentsInfo(DocumentsInfo info) {
    state = state.copyWith(documentsInfo: info);
    _saveDraft();
  }

  void updateAccountInfo(AccountInfo info) {
    state = state.copyWith(accountInfo: info);
    _saveDraft();
  }

  Future<void> _saveDraft() async {
    await _repository.saveDraft(state);
  }

  Future<String> submitRegistration() async {
    final result = await _submitUseCase(state);
    return result.fold((failure) => failure.message, (_) => 'Success');
  }

  Future<void> clearForm() async {
    await _repository.clearDraft();
    state = RegistrationForm.empty();
  }
}
