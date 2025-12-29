import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';
import '../../../../core/network/api_client.dart';
import '../../../../core/providers/core_providers.dart';
import '../../data/datasources/registration_local_datasource.dart';
import '../../data/datasources/registration_remote_datasource.dart';
import '../../data/repositories/registration_repository_impl.dart';
import '../../domain/repositories/registration_repository.dart';
import '../../domain/usecases/submit_registration.dart';
import '../viewmodels/section_state.dart';
import '../../data/repositories/image_picker_repository_imp.dart';

export '../viewmodels/registration_viewmodel.dart'
    show registrationViewModelProvider;

part 'registration_providers.g.dart';

final imagePickerProvider = Provider((ref) => ImagePickerRepositoryImp());
final pickProfilePhotoProvider = Provider(
  (ref) => ref.read(imagePickerProvider),
);

final registrationLocalDataSourceProvider =
    Provider<RegistrationLocalDataSource>((ref) {
      final sharedPreferences = ref.watch(sharedPreferencesProvider);
      return RegistrationLocalDataSourceImpl(
        sharedPreferences: sharedPreferences,
      );
    });

final registrationRemoteDataSourceProvider =
    Provider<RegistrationRemoteDataSource>((ref) {
      return RegistrationRemoteDataSourceImpl(apiClient: ApiClient());
    });

final registrationRepositoryProvider = Provider<RegistrationRepository>((ref) {
  final localDataSource = ref.watch(registrationLocalDataSourceProvider);
  final remoteDataSource = ref.watch(registrationRemoteDataSourceProvider);
  return RegistrationRepositoryImpl(
    localDataSource: localDataSource,
    remoteDataSource: remoteDataSource,
  );
});

final submitRegistrationUseCaseProvider = Provider<SubmitRegistration>((ref) {
  final repository = ref.watch(registrationRepositoryProvider);
  return SubmitRegistration(repository);
});

const int _totalRegistrationSteps = 4;
const int _maxStepIndex = _totalRegistrationSteps - 1;

@riverpod
class RegistrationFormStateNotifier extends _$RegistrationFormStateNotifier {
  @override
  RegistrationFormState build() {
    return RegistrationFormState.initial();
  }

  void nextStep() {
    if (state.currentStep < _maxStepIndex) {
      state = state.copyWith(currentStep: state.currentStep + 1);
    }
  }

  void previousStep() {
    if (state.currentStep > 0) {
      state = state.copyWith(currentStep: state.currentStep - 1);
    }
  }

  void goToStep(int step) {
    if (step >= 0 && step <= _maxStepIndex) {
      state = state.copyWith(currentStep: step);
    }
  }

  void setSubmitting(bool isSubmitting) {
    state = state.copyWith(isSubmitting: isSubmitting);
  }

  void setError(String? error) {
    state = state.copyWith(errorMessage: error, successMessage: null);
  }

  void setSuccess(String? message) {
    state = state.copyWith(successMessage: message, errorMessage: null);
  }

  void reset() {
    state = RegistrationFormState.initial();
  }
}
