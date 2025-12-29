import 'dart:typed_data';

class PickedFileData {
  final String name;
  final String path;
  final int size;
  final String? extension;
  final Uint8List? bytes;

  const PickedFileData({
    required this.name,
    required this.path,
    required this.size,
    this.extension,
    this.bytes,
  });

  double get sizeInMB => size / (1024 * 1024);

  bool isWithinSizeLimit(int limitMB) => sizeInMB <= limitMB;

  String get formattedSize {
    if (size < 1024) {
      return '$size B';
    } else if (size < 1024 * 1024) {
      return '${(size / 1024).toStringAsFixed(2)} KB';
    } else {
      return '${sizeInMB.toStringAsFixed(2)} MB';
    }
  }
}

enum FilePickerStatus { success, cancelled, error }

class FilePickerResult {
  final FilePickerStatus status;
  final PickedFileData? file;
  final String? errorMessage;

  const FilePickerResult({required this.status, this.file, this.errorMessage});

  factory FilePickerResult.success(PickedFileData file) {
    return FilePickerResult(status: FilePickerStatus.success, file: file);
  }

  factory FilePickerResult.cancelled() {
    return const FilePickerResult(status: FilePickerStatus.cancelled);
  }

  factory FilePickerResult.error(String message) {
    return FilePickerResult(
      status: FilePickerStatus.error,
      errorMessage: message,
    );
  }

  bool get isSuccess => status == FilePickerStatus.success && file != null;
  bool get isCancelled => status == FilePickerStatus.cancelled;
  bool get isError => status == FilePickerStatus.error;
}

abstract class FilePickerService {
  Future<FilePickerResult> pickFile({
    List<String>? allowedExtensions,
    int maxFileSizeMB = 10,
  });

  Future<List<FilePickerResult>> pickMultipleFiles({
    List<String>? allowedExtensions,
    int maxFileSizeMB = 10,
    int maxFiles = 5,
  });

  Future<FilePickerResult> pickImage({
    int maxFileSizeMB = 10,
    bool allowCamera = false,
  });
}
