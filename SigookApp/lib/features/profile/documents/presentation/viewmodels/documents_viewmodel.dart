import 'package:freezed_annotation/freezed_annotation.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';
import '../../../../../core/providers/analytics_providers.dart';
import '../../../presentation/providers/cached_worker_profile_provider.dart';
import '../providers/documents_providers.dart';

part 'documents_viewmodel.freezed.dart';
part 'documents_viewmodel.g.dart';

@freezed
abstract class DocumentsState with _$DocumentsState {
  const factory DocumentsState({
    @Default(false) bool isEditing,
    @Default(false) bool isSaving,
    String? saveError,
    @Default(false) bool justSaved,
  }) = _DocumentsState;
}

@riverpod
class DocumentsViewModel extends _$DocumentsViewModel {
  @override
  DocumentsState build() => const DocumentsState();

  void startEditing() => state = state.copyWith(
        isEditing: true,
        saveError: null,
        justSaved: false,
      );

  void cancelEditing() => state = state.copyWith(isEditing: false);

  Future<void> save(
    Map<String, String> fields, {
    Map<String, String>? filePaths,
  }) async {
    state = state.copyWith(isSaving: true, saveError: null, justSaved: false);

    final result = await ref.read(updateDocumentsUseCaseProvider)(
      fields,
      filePaths: filePaths,
    );

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
          parameters: {'section': 'documents'},
        );
      },
    );
  }
}
