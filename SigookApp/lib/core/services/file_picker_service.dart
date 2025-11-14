import 'dart:typed_data';

/// Represents a picked file with its metadata
class PickedFileData {
  final String name;
  final String path;
  final int size;
  final String? extension;
  final Uint8List? bytes; // For web platform

  const PickedFileData({
    required this.name,
    required this.path,
    required this.size,
    this.extension,
    this.bytes,
  });

  /// Get file size in MB
  double get sizeInMB => size / (1024 * 1024);

  /// Check if file size is within limit (in MB)
  bool isWithinSizeLimit(int limitMB) => sizeInMB <= limitMB;

  /// Get formatted file size
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

/// Enum for file picker result status
enum FilePickerStatus { success, cancelled, error }

/// Result of file picking operation
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

/// Service for picking files from device storage
/// Following SOLID principles - Interface Segregation Principle
abstract class FilePickerService {
  /// Pick a single file from device storage
  ///
  /// [allowedExtensions] - List of allowed file extensions (e.g., ['pdf', 'jpg', 'png'])
  /// [maxFileSizeMB] - Maximum file size in megabytes (default: 10MB)
  ///
  /// Returns [FilePickerResult] with the picked file or error/cancelled status
  Future<FilePickerResult> pickFile({
    List<String>? allowedExtensions,
    int maxFileSizeMB = 10,
  });

  /// Pick multiple files from device storage
  ///
  /// [allowedExtensions] - List of allowed file extensions
  /// [maxFileSizeMB] - Maximum file size per file in megabytes
  /// [maxFiles] - Maximum number of files to pick
  ///
  /// Returns list of [FilePickerResult]
  Future<List<FilePickerResult>> pickMultipleFiles({
    List<String>? allowedExtensions,
    int maxFileSizeMB = 10,
    int maxFiles = 5,
  });

  /// Pick an image file specifically
  /// Optimized for image selection with camera option on mobile
  ///
  /// [maxFileSizeMB] - Maximum file size in megabytes
  /// [allowCamera] - Allow camera capture on mobile devices
  Future<FilePickerResult> pickImage({
    int maxFileSizeMB = 10,
    bool allowCamera = false,
  });
}
