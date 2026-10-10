import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:sigook_app_flutter/core/providers/core_providers.dart';
import 'package:sigook_app_flutter/features/jobs/data/datasources/timesheet_remote_datasource.dart';
import 'package:sigook_app_flutter/features/jobs/data/repositories/timesheet_repository_impl.dart';
import 'package:sigook_app_flutter/features/jobs/domain/repositories/timesheet_repository.dart';
import 'package:sigook_app_flutter/features/jobs/domain/usecases/get_clock_type.dart';
import 'package:sigook_app_flutter/features/jobs/domain/usecases/submit_timesheet.dart';
import 'package:sigook_app_flutter/features/jobs/domain/usecases/get_timesheet_entries.dart';

final timesheetRemoteDatasourceProvider = Provider<TimesheetRemoteDatasource>(
  (ref) =>
      TimesheetRemoteDataSourceImpl(apiClient: ref.read(apiClientProvider)),
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

final submitTimesheetUseCaseProvider = Provider<SubmitTimesheet>(
  (ref) => SubmitTimesheet(ref.read(timesheetRemoteRepositoryProvider)),
);

final getTimesheetEntriesUseCaseProvider = Provider<GetTimesheetEntries>(
  (ref) => GetTimesheetEntries(ref.read(timesheetRemoteRepositoryProvider)),
);
