import 'package:dartz/dartz.dart';
import '../../../../../core/error/failures.dart';
import '../../../../../core/network/network_info.dart';
import '../../../data/repositories/activity_repository_helpers.dart';
import '../../domain/entities/week_time_sheets.dart';
import '../../domain/repositories/week_hours_repository.dart';
import '../datasources/week_hours_remote_datasource.dart';

class WeekHoursRepositoryImpl implements WeekHoursRepository {
  final WeekHoursRemoteDataSource datasource;
  final NetworkInfo networkInfo;

  WeekHoursRepositoryImpl({
    required this.datasource,
    required this.networkInfo,
  });

  @override
  Future<Either<Failure, WeekTimeSheets>> getWeek(DateTime weekStart) =>
      guardedActivityCall(networkInfo, () async => (await datasource.getWeek(weekStart)).toEntity());
}
