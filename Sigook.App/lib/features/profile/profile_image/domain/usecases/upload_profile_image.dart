import 'package:dartz/dartz.dart';
import '../../../../../core/error/failures.dart';
import '../repositories/profile_image_repository.dart';

class UploadProfileImage {
  final ProfileImageRepository repository;
  UploadProfileImage(this.repository);

  Future<Either<Failure, void>> call(String filePath) =>
      repository.upload(filePath);
}
