import 'package:freezed_annotation/freezed_annotation.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';
import '../../../presentation/providers/cached_worker_profile_provider.dart';
import '../providers/personal_details_providers.dart';

part 'personal_details_viewmodel.freezed.dart';
part 'personal_details_viewmodel.g.dart';

@freezed
abstract class PersonalDetailsState with _$PersonalDetailsState {
  const factory PersonalDetailsState({
    @Default(false) bool isEditing,
    @Default(false) bool isSaving,
    String? saveError,
    @Default(false) bool justSaved,
  }) = _PersonalDetailsState;
}

@riverpod
class PersonalDetailsViewModel extends _$PersonalDetailsViewModel {
  @override
  PersonalDetailsState build() => const PersonalDetailsState();

  void startEditing() => state = state.copyWith(
        isEditing: true,
        saveError: null,
        justSaved: false,
      );

  void cancelEditing() => state = state.copyWith(isEditing: false);

  Future<void> save(Map<String, String> fields) async {
    state = state.copyWith(isSaving: true, saveError: null, justSaved: false);

    final result =
        await ref.read(updatePersonalDetailsUseCaseProvider)(fields);

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
      },
    );
  }
}
