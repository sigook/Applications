import 'package:equatable/equatable.dart';
import 'package:uuid/uuid.dart';

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

  factory ProfileImage.fromPath(String localPath) {
    final fileName = localPath.split('/').last.split('\\').last;

    return ProfileImage(
      id: const Uuid().v4(),
      pathFile: localPath,
      fileName: fileName,
      description: 'Profile Photo',
      canDownload: true,
    );
  }

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
