// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'job_experience_viewmodel.dart';

// **************************************************************************
// RiverpodGenerator
// **************************************************************************

// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, type=warning

@ProviderFor(JobExperienceViewModel)
const jobExperienceViewModelProvider = JobExperienceViewModelProvider._();

final class JobExperienceViewModelProvider
    extends $NotifierProvider<JobExperienceViewModel, JobExperienceState> {
  const JobExperienceViewModelProvider._()
    : super(
        from: null,
        argument: null,
        retry: null,
        name: r'jobExperienceViewModelProvider',
        isAutoDispose: true,
        dependencies: null,
        $allTransitiveDependencies: null,
      );

  @override
  String debugGetCreateSourceHash() => _$jobExperienceViewModelHash();

  @$internal
  @override
  JobExperienceViewModel create() => JobExperienceViewModel();

  /// {@macro riverpod.override_with_value}
  Override overrideWithValue(JobExperienceState value) {
    return $ProviderOverride(
      origin: this,
      providerOverride: $SyncValueProvider<JobExperienceState>(value),
    );
  }
}

String _$jobExperienceViewModelHash() =>
    r'a1b2c3d4e5f6a7b8c9d0e1f2a3b4c5d6e7f8a9b0';

abstract class _$JobExperienceViewModel extends $Notifier<JobExperienceState> {
  JobExperienceState build();
  @$mustCallSuper
  @override
  void runBuild() {
    final created = build();
    final ref = this.ref as $Ref<JobExperienceState, JobExperienceState>;
    final element =
        ref.element
            as $ClassProviderElement<
              AnyNotifier<JobExperienceState, JobExperienceState>,
              JobExperienceState,
              Object?,
              Object?
            >;
    element.handleValue(ref, created);
  }
}
