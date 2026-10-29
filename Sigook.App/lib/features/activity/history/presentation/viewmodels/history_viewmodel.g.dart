// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'history_viewmodel.dart';

// **************************************************************************
// RiverpodGenerator
// **************************************************************************

// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, type=warning

@ProviderFor(AssignmentHistoryViewModel)
final assignmentHistoryViewModelProvider =
    AssignmentHistoryViewModelProvider._();

final class AssignmentHistoryViewModelProvider
    extends
        $NotifierProvider<AssignmentHistoryViewModel, AssignmentHistoryState> {
  AssignmentHistoryViewModelProvider._()
    : super(
        from: null,
        argument: null,
        retry: null,
        name: r'assignmentHistoryViewModelProvider',
        isAutoDispose: true,
        dependencies: null,
        $allTransitiveDependencies: null,
      );

  @override
  String debugGetCreateSourceHash() => _$assignmentHistoryViewModelHash();

  @$internal
  @override
  AssignmentHistoryViewModel create() => AssignmentHistoryViewModel();

  /// {@macro riverpod.override_with_value}
  Override overrideWithValue(AssignmentHistoryState value) {
    return $ProviderOverride(
      origin: this,
      providerOverride: $SyncValueProvider<AssignmentHistoryState>(value),
    );
  }
}

String _$assignmentHistoryViewModelHash() =>
    r'41ed74b681580b3ce52c53123676aa21eb8999e1';

abstract class _$AssignmentHistoryViewModel
    extends $Notifier<AssignmentHistoryState> {
  AssignmentHistoryState build();
  @$mustCallSuper
  @override
  WhenComplete runBuild() {
    final ref =
        this.ref as $Ref<AssignmentHistoryState, AssignmentHistoryState>;
    final element =
        ref.element
            as $ClassProviderElement<
              AnyNotifier<AssignmentHistoryState, AssignmentHistoryState>,
              AssignmentHistoryState,
              Object?,
              Object?
            >;
    return element.handleCreate(ref, build);
  }
}
