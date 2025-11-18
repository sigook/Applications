import 'package:file_picker/file_picker.dart' hide FilePickerResult;
import 'package:flutter/foundation.dart';
import 'file_picker_service.dart';

/// Implementation of FilePickerService using file_picker package
/// Following SOLID principles - Dependency Inversion Principle
/// Cross-platform support for iOS, Android, and Web
class FilePickerServiceImpl implements FilePickerService {
  final FilePicker _filePicker;

  FilePickerServiceImpl({FilePicker? filePicker})
    : _filePicker = filePicker ?? FilePicker.platform;

  @override
  Future<FilePickerResult> pickFile({
    List<String>? allowedExtensions,
    int maxFileSizeMB = 10,
  }) async {
    try {
      // Configure file type
      final FileType fileType =
          allowedExtensions != null && allowedExtensions.isNotEmpty
          ? FileType.custom
          : FileType.any;

      // Pick file
      final result = await _filePicker.pickFiles(
        type: fileType,
        allowedExtensions: allowedExtensions,
        allowMultiple: false,
        withData: kIsWeb, // Load bytes on web platform
        withReadStream: !kIsWeb, // Use stream on mobile for large files
      );

      // Handle cancellation
      if (result == null || result.files.isEmpty) {
        return FilePickerResult.cancelled();
      }

      final platformFile = result.files.first;

      // Validate file
      final validationError = _validateFile(
        platformFile,
        maxFileSizeMB,
        allowedExtensions,
      );
      if (validationError != null) {
        return FilePickerResult.error(validationError);
      }

      // Create PickedFileData
      final pickedFile = _createPickedFileData(platformFile);

      debugPrint(
        '✅ File picked successfully: ${pickedFile.name} (${pickedFile.formattedSize})',
      );

      return FilePickerResult.success(pickedFile);
    } catch (e) {
      debugPrint('❌ File picker error: ${e.toString()}');
      return FilePickerResult.error('Failed to pick file: ${e.toString()}');
    }
  }

  @override
  Future<List<FilePickerResult>> pickMultipleFiles({
    List<String>? allowedExtensions,
    int maxFileSizeMB = 10,
    int maxFiles = 5,
  }) async {
    try {
      // Configure file type
      final FileType fileType =
          allowedExtensions != null && allowedExtensions.isNotEmpty
          ? FileType.custom
          : FileType.any;

      // Pick files
      final result = await _filePicker.pickFiles(
        type: fileType,
        allowedExtensions: allowedExtensions,
        allowMultiple: true,
        withData: kIsWeb,
        withReadStream: !kIsWeb,
      );

      // Handle cancellation
      if (result == null || result.files.isEmpty) {
        return [FilePickerResult.cancelled()];
      }

      // Limit number of files
      final files = result.files.take(maxFiles).toList();

      // Process each file
      final results = <FilePickerResult>[];
      for (final platformFile in files) {
        // Validate file
        final validationError = _validateFile(
          platformFile,
          maxFileSizeMB,
          allowedExtensions,
        );
        if (validationError != null) {
          results.add(FilePickerResult.error(validationError));
          continue;
        }

        // Create PickedFileData
        final pickedFile = _createPickedFileData(platformFile);
        results.add(FilePickerResult.success(pickedFile));

        debugPrint(
          '✅ File picked: ${pickedFile.name} (${pickedFile.formattedSize})',
        );
      }

      return results;
    } catch (e) {
      debugPrint('❌ Multiple file picker error: ${e.toString()}');
      return [FilePickerResult.error('Failed to pick files: ${e.toString()}')];
    }
  }

  @override
  Future<FilePickerResult> pickImage({
    int maxFileSizeMB = 10,
    bool allowCamera = false,
  }) async {
    try {
      // For images, use FileType.image which is optimized for image selection
      final result = await _filePicker.pickFiles(
        type: FileType.image,
        allowMultiple: false,
        withData: kIsWeb,
        withReadStream: !kIsWeb,
      );

      // Handle cancellation
      if (result == null || result.files.isEmpty) {
        return FilePickerResult.cancelled();
      }

      final platformFile = result.files.first;

      // Validate file size
      final validationError = _validateFile(platformFile, maxFileSizeMB, [
        'jpg',
        'jpeg',
        'png',
        'gif',
        'bmp',
        'webp',
      ]);
      if (validationError != null) {
        return FilePickerResult.error(validationError);
      }

      // Create PickedFileData
      final pickedFile = _createPickedFileData(platformFile);

      debugPrint(
        '✅ Image picked: ${pickedFile.name} (${pickedFile.formattedSize})',
      );

      return FilePickerResult.success(pickedFile);
    } catch (e) {
      debugPrint('❌ Image picker error: ${e.toString()}');
      return FilePickerResult.error('Failed to pick image: ${e.toString()}');
    }
  }

  /// Validate file based on size and extension
  String? _validateFile(
    PlatformFile file,
    int maxFileSizeMB,
    List<String>? allowedExtensions,
  ) {
    // Check file size
    if (file.size > maxFileSizeMB * 1024 * 1024) {
      return 'File size exceeds ${maxFileSizeMB}MB limit';
    }

    // Check extension if specified
    if (allowedExtensions != null && allowedExtensions.isNotEmpty) {
      final extension = file.extension?.toLowerCase();
      if (extension == null || !allowedExtensions.contains(extension)) {
        return 'Invalid file type. Allowed: ${allowedExtensions.join(", ")}';
      }
    }

    return null; // Valid
  }

  /// Create PickedFileData from PlatformFile
  PickedFileData _createPickedFileData(PlatformFile platformFile) {
    return PickedFileData(
      name: platformFile.name,
      path: platformFile.path ?? '',
      size: platformFile.size,
      extension: platformFile.extension,
      bytes: platformFile.bytes, // Available on web
    );
  }
}
