/// Shared plain-Dart state for upload-only profile sections
/// (resume, licenses, certificates). Avoids 3 identical @freezed classes.
class UploadState {
  final bool isUploading;
  final String? uploadError;
  final bool justUploaded;

  const UploadState({
    this.isUploading = false,
    this.uploadError,
    this.justUploaded = false,
  });

  UploadState copyWith({
    bool? isUploading,
    String? uploadError,
    bool clearError = false,
    bool? justUploaded,
  }) =>
      UploadState(
        isUploading: isUploading ?? this.isUploading,
        uploadError: clearError ? null : (uploadError ?? this.uploadError),
        justUploaded: justUploaded ?? this.justUploaded,
      );
}
