import 'package:equatable/equatable.dart';

class ProfilePhoto extends Equatable {
  const ProfilePhoto({required this.path, this.isFromCamera = false});

  final String path;
  final bool isFromCamera;

  factory ProfilePhoto.empty() => const ProfilePhoto(path: '');

  bool get hasPhoto => path.isNotEmpty;

  /// Convert to JSON for debugging/logging purposes
  Map<String, dynamic> toJson() {
    return {'path': path, 'isFromCamera': isFromCamera};
  }

  @override
  List<Object?> get props => [path, isFromCamera];
}
