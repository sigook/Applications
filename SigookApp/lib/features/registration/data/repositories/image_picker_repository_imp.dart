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
    final permission = source == PhotoSource.camera
        ? Permission.camera
        : Permission.photos;

    final status = await permission.request();

    if (!status.isGranted) {
      return Left(PermissionFailure());
    }

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
  }
}
