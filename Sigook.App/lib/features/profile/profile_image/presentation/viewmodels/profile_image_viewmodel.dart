import 'package:flutter/foundation.dart';
import 'package:freezed_annotation/freezed_annotation.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';
import '../../../../../core/providers/analytics_providers.dart';
import '../../../presentation/providers/cached_worker_profile_provider.dart';
import '../providers/profile_image_providers.dart';

part 'profile_image_viewmodel.freezed.dart';
part 'profile_image_viewmodel.g.dart';

@freezed
abstract class ProfileImageState with _$ProfileImageState {
  const factory ProfileImageState({
    @Default(false) bool isUploading,
    String? uploadError,
    @Default(false) bool justUploaded,
  }) = _ProfileImageState;
}

@riverpod
class ProfileImageViewModel extends _$ProfileImageViewModel {
  @override
  ProfileImageState build() => const ProfileImageState();

  Future<void> upload(String filePath) async {
    state = state.copyWith(
      isUploading: true,
      uploadError: null,
      justUploaded: false,
    );

    debugPrint('📸 [PROFILE_IMAGE] Uploading profile image: $filePath');

    final result = await ref.read(uploadProfileImageUseCaseProvider)(filePath);

    result.fold(
      (failure) {
        debugPrint('📸 [PROFILE_IMAGE] Upload failed: ${failure.message}');
        state = state.copyWith(isUploading: false, uploadError: failure.message);
        ref.read(analyticsServiceProvider).logEvent(
          name: 'profile_image_upload_failed',
          parameters: {
            'error': failure.message,
            'timestamp': DateTime.now().toIso8601String(),
          },
        );
      },
      (_) {
        debugPrint('📸 [PROFILE_IMAGE] Upload successful');
        state = state.copyWith(isUploading: false, justUploaded: true);
        ref.invalidate(cachedWorkerProfileProvider);
        ref.read(analyticsServiceProvider).logEvent(
          name: 'profile_image_uploaded',
          parameters: {'timestamp': DateTime.now().toIso8601String()},
        );
      },
    );
  }
}
