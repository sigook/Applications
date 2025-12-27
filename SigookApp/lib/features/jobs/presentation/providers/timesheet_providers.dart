import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:sigook_app_flutter/core/providers/core_providers.dart';
import 'package:sigook_app_flutter/features/jobs/data/datasources/timesheet_remote_datasource.dart';
import 'package:sigook_app_flutter/features/jobs/data/repositories/timesheet_repository_impl.dart';
import 'package:sigook_app_flutter/features/jobs/domain/repositories/timesheet_repository.dart';
import 'package:sigook_app_flutter/features/jobs/domain/usecases/get_clock_type.dart';

final timesheetRemoteDatasourceProvider = Provider<TimesheetRemoteDatasource>(
  (ref) =>
      TimeSheetRemoteDataSourceImpl(apiClient: ref.read(apiClientProvider)),
);

final timesheetRemoteRepositoryProvider = Provider<TimesheetRepository>(
  (ref) => TimesheetRepositoryImpl(
    remoteDataSource: ref.read(timesheetRemoteDatasourceProvider),
    networkInfo: ref.read(networkInfoProvider),
  ),
);

final getClockTypeUseCaseProvider = Provider<GetClockType>(
  (ref) => GetClockType(ref.read(timesheetRemoteRepositoryProvider)),
);
