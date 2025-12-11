// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'jobs_viewmodel.dart';

// **************************************************************************
// RiverpodGenerator
// **************************************************************************

// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, type=warning

@ProviderFor(JobsViewModel)
const jobsViewModelProvider = JobsViewModelProvider._();

final class JobsViewModelProvider
    extends $NotifierProvider<JobsViewModel, JobsState> {
  const JobsViewModelProvider._()
    : super(
        from: null,
        argument: null,
        retry: null,
        name: r'jobsViewModelProvider',
        isAutoDispose: true,
        dependencies: null,
        $allTransitiveDependencies: null,
      );

  @override
  String debugGetCreateSourceHash() => _$jobsViewModelHash();

  @$internal
  @override
  JobsViewModel create() => JobsViewModel();

  /// {@macro riverpod.override_with_value}
  Override overrideWithValue(JobsState value) {
    return $ProviderOverride(
      origin: this,
      providerOverride: $SyncValueProvider<JobsState>(value),
    );
  }
}

String _$jobsViewModelHash() => r'2497a03c567b73f368a38d359ecfa2c8fb4b024f';

abstract class _$JobsViewModel extends $Notifier<JobsState> {
  JobsState build();
  @$mustCallSuper
  @override
  void runBuild() {
    final created = build();
    final ref = this.ref as $Ref<JobsState, JobsState>;
    final element =
        ref.element
            as $ClassProviderElement<
              AnyNotifier<JobsState, JobsState>,
              JobsState,
              Object?,
              Object?
            >;
    element.handleValue(ref, created);
  }
}
