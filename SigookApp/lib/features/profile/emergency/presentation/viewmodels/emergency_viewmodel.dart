import 'package:freezed_annotation/freezed_annotation.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';
import '../../../../../core/providers/analytics_providers.dart';
import '../../../presentation/providers/cached_worker_profile_provider.dart';
import '../providers/emergency_providers.dart';

part 'emergency_viewmodel.freezed.dart';
part 'emergency_viewmodel.g.dart';

@freezed
abstract class EmergencyState with _$EmergencyState {
  const factory EmergencyState({
    @Default(false) bool isEditing,
    @Default(false) bool isSaving,
    String? saveError,
    @Default(false) bool justSaved,
  }) = _EmergencyState;
}

@riverpod
class EmergencyViewModel extends _$EmergencyViewModel {
  @override
  EmergencyState build() => const EmergencyState();

  void startEditing() => state = state.copyWith(
        isEditing: true,
        saveError: null,
        justSaved: false,
      );

  void cancelEditing() => state = state.copyWith(isEditing: false);

  Future<void> save(Map<String, String> fields) async {
    state = state.copyWith(isSaving: true, saveError: null, justSaved: false);

    final result = await ref.read(updateEmergencyUseCaseProvider)(fields);

    result.fold(
      (failure) => state = state.copyWith(
        isSaving: false,
        saveError: 'Failed to save: ${failure.message}',
      ),
      (_) {
        state = state.copyWith(
          isSaving: false,
          isEditing: false,
          justSaved: true,
        );
        ref.invalidate(cachedWorkerProfileProvider);
        ref.read(analyticsServiceProvider).logEvent(
          name: 'profile_section_saved',
          parameters: {'section': 'emergency'},
        );
      },
    );
  }
}
