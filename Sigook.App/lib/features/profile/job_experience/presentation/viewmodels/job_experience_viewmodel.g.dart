// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'job_experience_viewmodel.dart';

// **************************************************************************
// RiverpodGenerator
// **************************************************************************

// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, type=warning

@ProviderFor(JobExperienceViewModel)
final jobExperienceViewModelProvider = JobExperienceViewModelProvider._();

final class JobExperienceViewModelProvider
    extends $NotifierProvider<JobExperienceViewModel, JobExperienceState> {
  JobExperienceViewModelProvider._()
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
    r'a1811dd72f9803a08b3f6235886b341ff4bfe471';

abstract class _$JobExperienceViewModel extends $Notifier<JobExperienceState> {
  JobExperienceState build();
  @$mustCallSuper
  @override
  WhenComplete runBuild() {
    final ref = this.ref as $Ref<JobExperienceState, JobExperienceState>;
    final element =
        ref.element
            as $ClassProviderElement<
              AnyNotifier<JobExperienceState, JobExperienceState>,
              JobExperienceState,
              Object?,
              Object?
            >;
    return element.handleCreate(ref, build);
  }
}
