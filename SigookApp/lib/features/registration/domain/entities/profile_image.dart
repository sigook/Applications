import 'package:equatable/equatable.dart';
import 'package:uuid/uuid.dart';

/// Profile image entity for API submission
class ProfileImage extends Equatable {
  final String id;
  final String pathFile;
  final String fileName;
  final String description;
  final bool canDownload;

  const ProfileImage({
    required this.id,
    required this.pathFile,
    required this.fileName,
    required this.description,
    this.canDownload = true,
  });

  /// Create from local file path
  factory ProfileImage.fromPath(String localPath) {
    // Extract filename from path
    final fileName = localPath.split('/').last.split('\\').last;

    return ProfileImage(
      id: const Uuid().v4(),
      pathFile: localPath,
      fileName: fileName,
      description: 'Profile Photo',
      canDownload: true,
    );
  }

  /// Convert to JSON for API
  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'pathFile': pathFile,
      'fileName': fileName,
      'description': description,
      'canDownload': canDownload,
    };
  }

  @override
  List<Object?> get props => [id, pathFile, fileName, description, canDownload];
}
