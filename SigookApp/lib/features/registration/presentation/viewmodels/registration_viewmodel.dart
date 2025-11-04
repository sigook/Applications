import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../domain/entities/personal_info.dart';
import '../../domain/entities/contact_info.dart';
import '../../domain/entities/address_info.dart';
import '../../domain/entities/availability_info.dart';
import '../../domain/entities/professional_info.dart';
import '../../domain/entities/registration_form.dart';
import '../../domain/repositories/registration_repository.dart';
import '../../domain/usecases/submit_registration.dart';

/// ViewModel for managing registration form state
class RegistrationViewModel extends StateNotifier<RegistrationForm> {
  final RegistrationRepository repository;
  final SubmitRegistration submitRegistrationUseCase;

  RegistrationViewModel({
    required this.repository,
    required this.submitRegistrationUseCase,
  }) : super(RegistrationForm.empty()) {
    _loadDraft();
  }

  /// Load saved draft
  Future<void> _loadDraft() async {
    final result = await repository.getDraft();
    result.fold(
      (failure) => {}, // Ignore error, start with empty form
      (form) => state = form,
    );
  }

  /// Update personal info section
  void updatePersonalInfo(PersonalInfo info) {
    state = state.copyWith(personalInfo: info);
    _saveDraft();
  }

  /// Update contact info section
  void updateContactInfo(ContactInfo info) {
    state = state.copyWith(contactInfo: info);
    _saveDraft();
  }

  /// Update address info section
  void updateAddressInfo(AddressInfo info) {
    state = state.copyWith(addressInfo: info);
    _saveDraft();
  }

  /// Update availability info section
  void updateAvailabilityInfo(AvailabilityInfo info) {
    state = state.copyWith(availabilityInfo: info);
    _saveDraft();
  }

  /// Update professional info section
  void updateProfessionalInfo(ProfessionalInfo info) {
    state = state.copyWith(professionalInfo: info);
    _saveDraft();
  }

  /// Save draft
  Future<void> _saveDraft() async {
    await repository.saveDraft(state);
  }

  /// Submit registration
  Future<bool> submitRegistration() async {
    final result = await submitRegistrationUseCase(state);
    return result.fold(
      (failure) => false,
      (_) => true,
    );
  }

  /// Check email availability
  Future<bool> checkEmailAvailability(String email) async {
    final result = await repository.isEmailAvailable(email);
    return result.fold(
      (failure) => false,
      (isAvailable) => isAvailable,
    );
  }

  /// Clear all form data
  Future<void> clearForm() async {
    await repository.clearDraft();
    state = RegistrationForm.empty();
  }
}
