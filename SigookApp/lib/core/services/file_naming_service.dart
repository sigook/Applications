import 'package:uuid/uuid.dart';
import 'dart:io';

class FileNamingService {
  static const _uuid = Uuid();

  static String _generateGuidWithoutDashes() {
    return _uuid.v4().replaceAll('-', '');
  }

  static String _getFileExtension(String filePath) {
    final file = File(filePath);
    final fileName = file.path.split(Platform.pathSeparator).last;
    final lastDot = fileName.lastIndexOf('.');
    return lastDot != -1 ? fileName.substring(lastDot) : '';
  }

  static String generateFileName(String prefix, String originalFilePath) {
    final guid = _generateGuidWithoutDashes();
    final extension = _getFileExtension(originalFilePath);
    return '${prefix}_$guid$extension';
  }

  static String generateProfileImageName(String originalFilePath) {
    return generateFileName('ProfileImage', originalFilePath);
  }

  static String generateDocumentName(String originalFilePath) {
    return generateFileName('Document', originalFilePath);
  }

  static String generateResumeName(String originalFilePath) {
    return generateFileName('Resume', originalFilePath);
  }

  static String generateLicenseName(String originalFilePath) {
    return generateFileName('License', originalFilePath);
  }

  static String generateCertificateName(String originalFilePath) {
    return generateFileName('Certificate', originalFilePath);
  }
}
