import 'package:freezed_annotation/freezed_annotation.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';
import '../../../../../core/providers/analytics_providers.dart';
import '../../../presentation/providers/cached_worker_profile_provider.dart';
import '../providers/licenses_providers.dart';

part 'licenses_viewmodel.freezed.dart';
part 'licenses_viewmodel.g.dart';

@freezed
abstract class LicensesState with _$LicensesState {
  const factory LicensesState({
    @Default(false) bool isUploading,
    String? uploadError,
    @Default(false) bool justUploaded,
    @Default(false) bool isDeleting,
    String? deleteError,
    @Default(false) bool justDeleted,
  }) = _LicensesState;
}

@riverpod
class LicensesViewModel extends _$LicensesViewModel {
  @override
  LicensesState build() => const LicensesState();

  Future<void> upload({
    required String filePath,
    required String number,
    required String issued,
    required String expires,
  }) async {
    state = state.copyWith(isUploading: true, uploadError: null, justUploaded: false);

    final result = await ref.read(uploadLicenseUseCaseProvider)(
      filePath: filePath,
      number: number,
      issued: issued,
      expires: expires,
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
          parameters: {'section': 'licenses'},
        );
      },
    );
  }

  Future<void> delete(String licenseId) async {
    state = state.copyWith(isDeleting: true, deleteError: null, justDeleted: false);

    final result = await ref.read(deleteLicenseUseCaseProvider)(licenseId);

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
