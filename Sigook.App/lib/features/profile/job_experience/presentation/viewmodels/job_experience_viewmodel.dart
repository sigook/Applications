import 'package:freezed_annotation/freezed_annotation.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';
import '../../../../../core/providers/analytics_providers.dart';
import '../providers/job_experience_providers.dart';

part 'job_experience_viewmodel.freezed.dart';
part 'job_experience_viewmodel.g.dart';

@freezed
abstract class JobExperienceState with _$JobExperienceState {
  const factory JobExperienceState({
    // Add flow
    @Default(false) bool isAdding,
    String? addError,
    @Default(false) bool justAdded,
    @Default(false) bool showForm,
    // Edit flow
    String? editingId,
    @Default(false) bool isSaving,
    String? saveError,
    @Default(false) bool justSaved,
    // Delete flow
    String? deletingId,
    String? deleteError,
    @Default(false) bool justDeleted,
  }) = _JobExperienceState;
}

@riverpod
class JobExperienceViewModel extends _$JobExperienceViewModel {
  @override
  JobExperienceState build() => const JobExperienceState();

  void startAdding() => state = state.copyWith(showForm: true, addError: null);

  void cancelAdding() =>
      state = state.copyWith(showForm: false, addError: null, justAdded: false);

  void startEditing(String id) =>
      state = state.copyWith(editingId: id, saveError: null, justSaved: false);

  void cancelEditing() => state = state.copyWith(editingId: null);

  Future<void> add({
    required String company,
    String? supervisor,
    String? duties,
    required String startDate,
    String? endDate,
    required bool isCurrentJobPosition,
  }) async {
    state = state.copyWith(isAdding: true, addError: null, justAdded: false);

    final result = await ref.read(addJobExperienceUseCaseProvider)(
      company: company,
      supervisor: supervisor,
      duties: duties,
      startDate: startDate,
      endDate: endDate,
      isCurrentJobPosition: isCurrentJobPosition,
    );

    result.fold(
      (failure) =>
          state = state.copyWith(isAdding: false, addError: failure.message),
      (_) {
        state = state.copyWith(isAdding: false, justAdded: true, showForm: false);
        ref.invalidate(jobExperienceListProvider);
        ref.read(analyticsServiceProvider).logEvent(
          name: 'profile_section_saved',
          parameters: {'section': 'job_experience'},
        );
      },
    );
  }

  Future<void> update({
    required String id,
    required String company,
    String? supervisor,
    String? duties,
    required String startDate,
    String? endDate,
    required bool isCurrentJobPosition,
  }) async {
    state = state.copyWith(isSaving: true, saveError: null, justSaved: false);

    final result = await ref.read(updateJobExperienceUseCaseProvider)(
      id: id,
      company: company,
      supervisor: supervisor,
      duties: duties,
      startDate: startDate,
      endDate: endDate,
      isCurrentJobPosition: isCurrentJobPosition,
    );

    result.fold(
      (failure) =>
          state = state.copyWith(isSaving: false, saveError: failure.message),
      (_) {
        state = state.copyWith(
          isSaving: false,
          editingId: null,
          justSaved: true,
        );
        ref.invalidate(jobExperienceListProvider);
        ref.read(analyticsServiceProvider).logEvent(
          name: 'profile_section_saved',
          parameters: {'section': 'job_experience'},
        );
      },
    );
  }

  Future<void> delete(String id) async {
    state = state.copyWith(
      deletingId: id,
      deleteError: null,
      justDeleted: false,
    );

    final result = await ref.read(deleteJobExperienceUseCaseProvider)(id);

    result.fold(
      (failure) => state = state.copyWith(
        deletingId: null,
        deleteError: failure.message,
      ),
      (_) {
        state = state.copyWith(deletingId: null, justDeleted: true);
        ref.invalidate(jobExperienceListProvider);
      },
    );
  }
}
