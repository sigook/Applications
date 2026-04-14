// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'resume_viewmodel.dart';

// **************************************************************************
// RiverpodGenerator
// **************************************************************************

// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, type=warning

@ProviderFor(ResumeViewModel)
const resumeViewModelProvider = ResumeViewModelProvider._();

final class ResumeViewModelProvider
    extends $NotifierProvider<ResumeViewModel, ResumeState> {
  const ResumeViewModelProvider._()
    : super(
        from: null,
        argument: null,
        retry: null,
        name: r'resumeViewModelProvider',
        isAutoDispose: true,
        dependencies: null,
        $allTransitiveDependencies: null,
      );

  @override
  String debugGetCreateSourceHash() => _$resumeViewModelHash();

  @$internal
  @override
  ResumeViewModel create() => ResumeViewModel();

  /// {@macro riverpod.override_with_value}
  Override overrideWithValue(ResumeState value) {
    return $ProviderOverride(
      origin: this,
      providerOverride: $SyncValueProvider<ResumeState>(value),
    );
  }
}

String _$resumeViewModelHash() => r'e5c27bb6db6562c644ed87aa2d82dc63b293810b';

abstract class _$ResumeViewModel extends $Notifier<ResumeState> {
  ResumeState build();
  @$mustCallSuper
  @override
  void runBuild() {
    final created = build();
    final ref = this.ref as $Ref<ResumeState, ResumeState>;
    final element =
        ref.element
            as $ClassProviderElement<
              AnyNotifier<ResumeState, ResumeState>,
              ResumeState,
              Object?,
              Object?
            >;
    element.handleValue(ref, created);
  }
}
