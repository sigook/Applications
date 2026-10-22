import 'package:freezed_annotation/freezed_annotation.dart';
import '../../domain/entities/job.dart';
import '../../domain/entities/jobs_filter.dart';

part 'jobs_state.freezed.dart';

@freezed
abstract class JobsState with _$JobsState {
  const factory JobsState({
    @Default([]) List<Job> jobs,
    @Default(false) bool isLoading,
    @Default(false) bool isLoadingMore,
    String? error,
    @Default(1) int currentPage,
    @Default(true) bool hasMore,
    @Default(JobsFilter.defaults) JobsFilter filter,
  }) = _JobsState;
}
