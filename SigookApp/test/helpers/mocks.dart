import 'package:mocktail/mocktail.dart';
import 'package:sigook_app_flutter/core/network/network_info.dart';
import 'package:sigook_app_flutter/core/services/analytics_service.dart';
import 'package:sigook_app_flutter/core/services/crash_reporting_service.dart';
import 'package:sigook_app_flutter/features/auth/data/datasources/auth_local_datasource.dart';
import 'package:sigook_app_flutter/features/auth/data/datasources/auth_remote_datasource.dart';
import 'package:sigook_app_flutter/features/auth/domain/repositories/auth_repository.dart';
import 'package:sigook_app_flutter/features/jobs/data/datasources/jobs_local_datasource.dart';
import 'package:sigook_app_flutter/features/jobs/data/datasources/jobs_remote_datasource.dart';
import 'package:sigook_app_flutter/features/jobs/domain/repositories/jobs_repository.dart';
import 'package:sigook_app_flutter/features/jobs/domain/repositories/timesheet_repository.dart';
import 'package:sigook_app_flutter/features/profile/domain/repositories/profile_repository.dart';
import 'package:sigook_app_flutter/features/profile/personal_details/domain/repositories/personal_details_repository.dart';

// ── Infrastructure ──────────────────────────────────────────────────────────
class MockNetworkInfo extends Mock implements NetworkInfo {}

class MockAnalyticsService extends Mock implements AnalyticsService {}

class MockCrashReportingService extends Mock implements CrashReportingService {}

// ── Auth ────────────────────────────────────────────────────────────────────
class MockAuthRepository extends Mock implements AuthRepository {}

class MockAuthRemoteDataSource extends Mock implements AuthRemoteDataSource {}

class MockAuthLocalDataSource extends Mock implements AuthLocalDataSource {}

// ── Jobs ────────────────────────────────────────────────────────────────────
class MockJobsRepository extends Mock implements JobsRepository {}

class MockTimesheetRepository extends Mock implements TimesheetRepository {}

class MockJobsRemoteDataSource extends Mock implements JobsRemoteDataSource {}

class MockJobsLocalDataSource extends Mock implements JobsLocalDataSource {}

// ── Profile ─────────────────────────────────────────────────────────────────
class MockProfileRepository extends Mock implements ProfileRepository {}

class MockPersonalDetailsRepository extends Mock
    implements PersonalDetailsRepository {}
