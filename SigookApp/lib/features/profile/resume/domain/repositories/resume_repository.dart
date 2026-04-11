import 'package:dartz/dartz.dart';
import '../../../../../core/error/failures.dart';

abstract class ResumeRepository {
  Future<Either<Failure, void>> upload(String filePath);
}
