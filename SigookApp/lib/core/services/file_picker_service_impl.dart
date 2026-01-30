import 'package:file_picker/file_picker.dart' hide FilePickerResult;
import 'package:flutter/foundation.dart';
import 'file_picker_service.dart';

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
      final FileType fileType =
          allowedExtensions != null && allowedExtensions.isNotEmpty
          ? FileType.custom
          : FileType.any;

      final result = await _filePicker.pickFiles(
        type: fileType,
        allowedExtensions: allowedExtensions,
        allowMultiple: false,
        withData: kIsWeb,
        withReadStream: !kIsWeb,
      );

      if (result == null || result.files.isEmpty) {
        return FilePickerResult.cancelled();
      }

      final platformFile = result.files.first;

      final validationError = _validateFile(
        platformFile,
        maxFileSizeMB,
        allowedExtensions,
      );
      if (validationError != null) {
        return FilePickerResult.error(validationError);
      }

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
      final FileType fileType =
          allowedExtensions != null && allowedExtensions.isNotEmpty
          ? FileType.custom
          : FileType.any;

      final result = await _filePicker.pickFiles(
        type: fileType,
        allowedExtensions: allowedExtensions,
        allowMultiple: true,
        withData: kIsWeb,
        withReadStream: !kIsWeb,
      );

      if (result == null || result.files.isEmpty) {
        return [FilePickerResult.cancelled()];
      }

      final files = result.files.take(maxFiles).toList();

      final results = <FilePickerResult>[];
      for (final platformFile in files) {
        final validationError = _validateFile(
          platformFile,
          maxFileSizeMB,
          allowedExtensions,
        );
        if (validationError != null) {
          results.add(FilePickerResult.error(validationError));
          continue;
        }

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
      final result = await _filePicker.pickFiles(
        type: FileType.image,
        allowMultiple: false,
        withData: kIsWeb,
        withReadStream: !kIsWeb,
      );

      if (result == null || result.files.isEmpty) {
        return FilePickerResult.cancelled();
      }

      final platformFile = result.files.first;

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

  String? _validateFile(
    PlatformFile file,
    int maxFileSizeMB,
    List<String>? allowedExtensions,
  ) {
    if (file.size > maxFileSizeMB * 1024 * 1024) {
      return 'File size exceeds ${maxFileSizeMB}MB limit';
    }

    if (allowedExtensions != null && allowedExtensions.isNotEmpty) {
      final extension = file.extension?.toLowerCase();
      if (extension == null || !allowedExtensions.contains(extension)) {
        return 'Invalid file type. Allowed: ${allowedExtensions.join(", ")}';
      }
    }

    return null;
  }

  PickedFileData _createPickedFileData(PlatformFile platformFile) {
    return PickedFileData(
      name: platformFile.name,
      path: platformFile.path ?? '',
      size: platformFile.size,
      extension: platformFile.extension,
      bytes: platformFile.bytes,
    );
  }
}
