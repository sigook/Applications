import 'package:dartz/dartz.dart';
import '../../../../../core/error/failures.dart';
import '../entities/week_time_sheets.dart';

abstract class WeekHoursRepository {
  Future<Either<Failure, WeekTimeSheets>> getWeek(DateTime weekStart);
}
