import 'package:dartz/dartz.dart';
import '../../../../../core/error/failures.dart';

abstract class ProfileImageRepository {
  Future<Either<Failure, void>> upload(String filePath);
}
