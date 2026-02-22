// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'history_viewmodel.dart';

// **************************************************************************
// RiverpodGenerator
// **************************************************************************

// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, type=warning

@ProviderFor(HistoryViewModel)
const historyViewModelProvider = HistoryViewModelProvider._();

final class HistoryViewModelProvider
    extends $NotifierProvider<HistoryViewModel, HistoryState> {
  const HistoryViewModelProvider._()
    : super(
        from: null,
        argument: null,
        retry: null,
        name: r'historyViewModelProvider',
        isAutoDispose: true,
        dependencies: null,
        $allTransitiveDependencies: null,
      );

  @override
  String debugGetCreateSourceHash() => _$historyViewModelHash();

  @$internal
  @override
  HistoryViewModel create() => HistoryViewModel();

  /// {@macro riverpod.override_with_value}
  Override overrideWithValue(HistoryState value) {
    return $ProviderOverride(
      origin: this,
      providerOverride: $SyncValueProvider<HistoryState>(value),
    );
  }
}

String _$historyViewModelHash() => r'cdf8d009510726ad7d097129a0aeb8f999d03a71';

abstract class _$HistoryViewModel extends $Notifier<HistoryState> {
  HistoryState build();
  @$mustCallSuper
  @override
  void runBuild() {
    final created = build();
    final ref = this.ref as $Ref<HistoryState, HistoryState>;
    final element =
        ref.element
            as $ClassProviderElement<
              AnyNotifier<HistoryState, HistoryState>,
              HistoryState,
              Object?,
              Object?
            >;
    element.handleValue(ref, created);
  }
}
