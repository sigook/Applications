import 'package:freezed_annotation/freezed_annotation.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';
import '../../../../../core/providers/analytics_providers.dart';
import '../../../presentation/providers/cached_worker_profile_provider.dart';
import '../providers/other_documents_providers.dart';

part 'other_documents_viewmodel.freezed.dart';
part 'other_documents_viewmodel.g.dart';

@freezed
abstract class OtherDocumentsState with _$OtherDocumentsState {
  const factory OtherDocumentsState({
    @Default(false) bool isUploading,
    String? uploadError,
    @Default(false) bool justUploaded,
    @Default(false) bool isDeleting,
    String? deleteError,
    @Default(false) bool justDeleted,
  }) = _OtherDocumentsState;
}

@riverpod
class OtherDocumentsViewModel extends _$OtherDocumentsViewModel {
  @override
  OtherDocumentsState build() => const OtherDocumentsState();

  Future<void> upload({
    required String filePath,
    required String description,
  }) async {
    state = state.copyWith(isUploading: true, uploadError: null, justUploaded: false);

    final result = await ref.read(uploadOtherDocumentUseCaseProvider)(
      filePath: filePath,
      description: description,
    );

    result.fold(
      (failure) => state = state.copyWith(
        isUploading: false,
        uploadError: failure.message,
      ),
      (_) {
        state = state.copyWith(isUploading: false, justUploaded: true);
        ref.invalidate(cachedWorkerProfileProvider);
        ref.read(analyticsServiceProvider).logEvent(
          name: 'profile_section_saved',
          parameters: {'section': 'other_documents'},
        );
      },
    );
  }

  Future<void> delete(String documentId) async {
    state = state.copyWith(isDeleting: true, deleteError: null, justDeleted: false);

    final result = await ref.read(deleteOtherDocumentUseCaseProvider)(documentId);

    result.fold(
      (failure) => state = state.copyWith(
        isDeleting: false,
        deleteError: failure.message,
      ),
      (_) {
        state = state.copyWith(isDeleting: false, justDeleted: true);
        ref.invalidate(cachedWorkerProfileProvider);
      },
    );
  }
}
