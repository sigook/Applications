import 'package:freezed_annotation/freezed_annotation.dart';

part 'section_state.freezed.dart';

/// State for individual form sections
@freezed
sealed class SectionState<T> with _$SectionState<T> {
  const factory SectionState.initial() = _Initial<T>;
  const factory SectionState.editing(T data) = _Editing<T>;
  const factory SectionState.validating() = _Validating<T>;
  const factory SectionState.valid(T data) = _Valid<T>;
  const factory SectionState.invalid(T data, String error) = _Invalid<T>;
}

/// Overall form state
@freezed
sealed class RegistrationFormState with _$RegistrationFormState {
  const factory RegistrationFormState({
    required int currentStep,
    required bool isSubmitting,
    String? errorMessage,
    String? successMessage,
  }) = _RegistrationFormState;

  factory RegistrationFormState.initial() =>
      const RegistrationFormState(currentStep: 0, isSubmitting: false);
}
