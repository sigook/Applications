import 'dart:io';
import 'package:image_picker/image_picker.dart';
import 'package:permission_handler/permission_handler.dart';
import 'package:sigook_app_flutter/core/error/failures.dart';
import 'package:sigook_app_flutter/features/registration/domain/entities/value_objects/profile_photo.dart';
import 'package:sigook_app_flutter/features/registration/domain/usecases/pick_profile_photo.dart';
import 'package:dartz/dartz.dart';

class ImagePickerRepositoryImp implements PickProfilePhoto {
  final ImagePicker _picker = ImagePicker();

  @override
  Future<Either<Failure, ProfilePhoto>> call(PhotoSource source) async {
    // For camera, we need to request permission explicitly
    // For gallery, image_picker uses the system photo picker which doesn't require permission
    if (source == PhotoSource.camera) {
      final status = await Permission.camera.request();
      if (!status.isGranted) {
        // On iOS, check if permanently denied to show settings prompt
        if (Platform.isIOS && status.isPermanentlyDenied) {
          return Left(PermissionFailure());
        }
        if (!status.isGranted) {
          return Left(PermissionFailure());
        }
      }
    }
    // Note: For gallery on Android 13+, the system photo picker is used (no permission needed)
    // For gallery on iOS, image_picker handles the permission dialog internally

    try {
      final XFile? file = await _picker.pickImage(
        imageQuality: 85,
        maxWidth: 1200,
        source: source == PhotoSource.camera
            ? ImageSource.camera
            : ImageSource.gallery,
      );

      if (file == null) {
        return Left(UserCancelledFailure());
      }

      return Right(
        ProfilePhoto(path: file.path, isFromCamera: source == PhotoSource.camera),
      );
    } catch (e) {
      // Handle platform exceptions (e.g., permission denied by system)
      if (e.toString().contains('permission') ||
          e.toString().contains('denied') ||
          e.toString().contains('photo_access_denied')) {
        return Left(PermissionFailure());
      }
      return Left(ServerFailure());
    }
  }
}
