// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'hours_viewmodel.dart';

// **************************************************************************
// RiverpodGenerator
// **************************************************************************

// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, type=warning

@ProviderFor(HoursViewModel)
final hoursViewModelProvider = HoursViewModelProvider._();

final class HoursViewModelProvider
    extends $NotifierProvider<HoursViewModel, HoursState> {
  HoursViewModelProvider._()
    : super(
        from: null,
        argument: null,
        retry: null,
        name: r'hoursViewModelProvider',
        isAutoDispose: true,
        dependencies: null,
        $allTransitiveDependencies: null,
      );

  @override
  String debugGetCreateSourceHash() => _$hoursViewModelHash();

  @$internal
  @override
  HoursViewModel create() => HoursViewModel();

  /// {@macro riverpod.override_with_value}
  Override overrideWithValue(HoursState value) {
    return $ProviderOverride(
      origin: this,
      providerOverride: $SyncValueProvider<HoursState>(value),
    );
  }
}

String _$hoursViewModelHash() => r'c4fd4516ae51a41d40eb30117d1cf4967326f7ee';

abstract class _$HoursViewModel extends $Notifier<HoursState> {
  HoursState build();
  @$mustCallSuper
  @override
  WhenComplete runBuild() {
    final ref = this.ref as $Ref<HoursState, HoursState>;
    final element =
        ref.element
            as $ClassProviderElement<
              AnyNotifier<HoursState, HoursState>,
              HoursState,
              Object?,
              Object?
            >;
    return element.handleCreate(ref, build);
  }
}
