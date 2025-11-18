import 'package:dartz/dartz.dart';
import '../../../../core/error/failures.dart';
import '../../../../core/usecases/usecase.dart';

/// Parameters for section validation
class ValidateSectionParams {
  final String sectionName;
  final dynamic sectionData;

  ValidateSectionParams({required this.sectionName, required this.sectionData});
}

/// Use case for validating individual form sections
class ValidateSection implements UseCase<bool, ValidateSectionParams> {
  @override
  Future<Either<Failure, bool>> call(ValidateSectionParams params) async {
    try {
      // Validate based on section type
      final isValid = _validateSection(params.sectionData);

      if (!isValid) {
        return Left(
          ValidationFailure(message: '${params.sectionName} validation failed'),
        );
      }

      return const Right(true);
    } catch (e) {
      return Left(ValidationFailure(message: e.toString()));
    }
  }

  bool _validateSection(dynamic sectionData) {
    // Check if section data has isValid method
    if (sectionData == null) return false;

    try {
      return (sectionData as dynamic).isValid as bool;
    } catch (e) {
      return false;
    }
  }
}
