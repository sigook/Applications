import 'package:freezed_annotation/freezed_annotation.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';
import '../../../../../core/providers/analytics_providers.dart';
import '../../../presentation/providers/cached_worker_profile_provider.dart';
import '../providers/certificates_providers.dart';

part 'certificates_viewmodel.freezed.dart';
part 'certificates_viewmodel.g.dart';

@freezed
abstract class CertificatesState with _$CertificatesState {
  const factory CertificatesState({
    @Default(false) bool isUploading,
    String? uploadError,
    @Default(false) bool justUploaded,
    @Default(false) bool isDeleting,
    String? deleteError,
    @Default(false) bool justDeleted,
  }) = _CertificatesState;
}

@riverpod
class CertificatesViewModel extends _$CertificatesViewModel {
  @override
  CertificatesState build() => const CertificatesState();

  Future<void> upload(String filePath) async {
    state = state.copyWith(isUploading: true, uploadError: null, justUploaded: false);

    final result = await ref.read(uploadCertificateUseCaseProvider)(filePath);

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
          parameters: {'section': 'certificates'},
        );
      },
    );
  }

  Future<void> delete(String certificateId) async {
    state = state.copyWith(isDeleting: true, deleteError: null, justDeleted: false);

    final result = await ref.read(deleteCertificateUseCaseProvider)(certificateId);

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
