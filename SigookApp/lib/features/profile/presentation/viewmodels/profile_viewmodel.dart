import 'package:freezed_annotation/freezed_annotation.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';
import '../../domain/usecases/update_worker_profile.dart';
import '../providers/cached_worker_profile_provider.dart';
import '../providers/profile_providers.dart';

part 'profile_viewmodel.freezed.dart';
part 'profile_viewmodel.g.dart';

@freezed
abstract class ProfileState with _$ProfileState {
  const factory ProfileState({
    @Default(false) bool isSaving,
    ProfileSection? editingSection,
    /// Non-null when a save failed — consumed by ref.listen in the page.
    String? saveError,
    /// Flips to true after a successful save — consumed by ref.listen in the page.
    @Default(false) bool justSaved,
  }) = _ProfileState;
}

@riverpod
class ProfileViewModel extends _$ProfileViewModel {
  @override
  ProfileState build() => const ProfileState();

  void startEditing(ProfileSection section) {
    state = state.copyWith(
      editingSection: section,
      saveError: null,
      justSaved: false,
    );
  }

  void cancelEditing() {
    state = state.copyWith(editingSection: null);
  }

  Future<void> saveSection(
    ProfileSection section,
    Map<String, String> fields, {
    Map<String, String>? filePaths,
  }) async {
    state = state.copyWith(isSaving: true, saveError: null, justSaved: false);

    final result = await ref.read(updateWorkerProfileUseCaseProvider)(
      UpdateWorkerProfileParams(
        editedFields: fields,
        section: section,
        newFilePaths: filePaths?.isNotEmpty == true ? filePaths : null,
      ),
    );

    result.fold(
      (failure) => state = state.copyWith(
        isSaving: false,
        saveError: 'Failed to save: ${failure.message}',
      ),
      (_) {
        state = state.copyWith(
          isSaving: false,
          editingSection: null,
          justSaved: true,
          saveError: null,
        );
        ref.invalidate(cachedWorkerProfileProvider);
      },
    );
  }

  /// Called by upload sections (Resume, Licenses, Certificates) which manage
  /// their own file state but delegate the actual API call here.
  Future<String?> uploadFile(
    ProfileSection section,
    Map<String, String> fields,
    Map<String, String> filePaths,
  ) async {
    final result = await ref.read(updateWorkerProfileUseCaseProvider)(
      UpdateWorkerProfileParams(
        editedFields: fields,
        section: section,
        newFilePaths: filePaths,
      ),
    );

    return result.fold(
      (failure) => failure.message,
      (_) {
        ref.invalidate(cachedWorkerProfileProvider);
        return null; // null = success
      },
    );
  }
}
