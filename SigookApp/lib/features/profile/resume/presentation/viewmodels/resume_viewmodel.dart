import 'package:freezed_annotation/freezed_annotation.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';
import '../../../../../core/providers/analytics_providers.dart';
import '../../../presentation/providers/cached_worker_profile_provider.dart';
import '../providers/resume_providers.dart';

part 'resume_viewmodel.freezed.dart';
part 'resume_viewmodel.g.dart';

@freezed
abstract class ResumeState with _$ResumeState {
  const factory ResumeState({
    @Default(false) bool isUploading,
    String? uploadError,
    @Default(false) bool justUploaded,
  }) = _ResumeState;
}

@riverpod
class ResumeViewModel extends _$ResumeViewModel {
  @override
  ResumeState build() => const ResumeState();

  Future<void> upload(String filePath) async {
    state = state.copyWith(isUploading: true, uploadError: null, justUploaded: false);

    final result = await ref.read(uploadResumeUseCaseProvider)(filePath);

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
          parameters: {'section': 'resume'},
        );
      },
    );
  }
}
