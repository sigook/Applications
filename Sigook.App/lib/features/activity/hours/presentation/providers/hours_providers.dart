import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../../../../core/providers/core_providers.dart';
import '../../../../auth/presentation/providers/auth_providers.dart';
import '../../data/datasources/week_hours_remote_datasource.dart';
import '../../data/repositories/week_hours_repository_impl.dart';
import '../../domain/repositories/week_hours_repository.dart';
import '../../domain/usecases/get_week_time_sheets.dart';

final weekHoursDatasourceProvider = Provider<WeekHoursRemoteDataSource>((ref) {
  return WeekHoursRemoteDataSourceImpl(
    apiClient: ref.read(authenticatedApiClientProvider),
  );
});

final weekHoursRepositoryProvider = Provider<WeekHoursRepository>((ref) {
  return WeekHoursRepositoryImpl(
    datasource: ref.read(weekHoursDatasourceProvider),
    networkInfo: ref.read(networkInfoProvider),
  );
});

final getWeekTimeSheetsUseCaseProvider = Provider<GetWeekTimeSheets>((ref) {
  return GetWeekTimeSheets(ref.read(weekHoursRepositoryProvider));
});
