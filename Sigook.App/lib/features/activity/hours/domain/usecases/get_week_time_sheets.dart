import 'package:dartz/dartz.dart';
import '../../../../../core/error/failures.dart';
import '../entities/week_time_sheets.dart';
import '../repositories/week_hours_repository.dart';

class GetWeekTimeSheets {
  final WeekHoursRepository repository;

  GetWeekTimeSheets(this.repository);

  Future<Either<Failure, WeekTimeSheets>> call(DateTime weekStart) =>
      repository.getWeek(weekStart);
}
