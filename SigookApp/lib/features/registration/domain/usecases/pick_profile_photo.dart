import 'package:sigook_app_flutter/core/error/failures.dart';
import 'package:sigook_app_flutter/features/registration/domain/entities/value_objects/profile_photo.dart';
import 'package:dartz/dartz.dart';

enum PhotoSource { camera, gallery }

abstract class PickProfilePhoto {
  Future<Either<Failure, ProfilePhoto>> call(PhotoSource source);
}
